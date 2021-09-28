﻿using ApiBase.Data;
using ApiBase.Models;
using ApiBase.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiBase.Controllers
{
    [Authorize]
    public class GenericController<T> : ControllerBase where T : class, IEntity<T>
    {
        protected readonly UserManager<IdentityUser> _userManager;
        protected readonly ApiDbContext _database;
        protected ILogger<GenericController<T>> _logger;
        protected readonly IHubContext<EventHub> _hub;


        public GenericController(UserManager<IdentityUser> userManager, ApiDbContext database, ILogger<GenericController<T>> logger, IHubContext<EventHub> hub)
        {
            _userManager = userManager;
            _database = database;
            _logger = logger;
            _hub = hub;
        }

        /// <summary>
        /// visszaadja az adott user összes T-jét
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetMyTs()
        {
            var myself = CurrentUserId();
            return new JsonResult(_database.Set<T>().Where(t => t.OwnerId == myself));
        }

        /// <summary>
        /// visszaadja az adott user adott id-jű T-jét
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public JsonResult GetMyT(string id)
        {
            var myself = CurrentUserId();
            return new JsonResult(_database.Set<T>().FirstOrDefault(t => t.OwnerId == myself && t.Id == id));
        }

        /// <summary>
        /// visszaadja minden user összes T-ját (csak adminoknak!)
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPatch]
        public JsonResult GetAllT()
        {
            return new JsonResult(_database.Set<T>());
        }

        /// <summary>
        /// visszaadja valamely user konkrét T-ját (csak adminoknak!)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        public JsonResult GetTFromUser(string id)
        {
            return new JsonResult(_database.Set<T>().FirstOrDefault(t => t.Id == id));
        }

        /// <summary>
        /// T létrehozása
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<JsonResult> CreateT([FromBody] T value)
        {
            value.OwnerId = CurrentUserId();
            _database.Set<T>().Add(value);
            _database.SaveChanges();

            await _hub.Clients.All.SendAsync(value.GetType().Name + "Added", value);

            return new JsonResult(Ok(_database.Set<T>().FirstOrDefault(t => t.Id == value.Id)));
        }

        /// <summary>
        /// T törlése id alapján, ha a miénk vagy adminok vagyunk
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<JsonResult> RemoveT(string id)
        {
            var myself = CurrentUserId();
            var myselfRoles = await _userManager.GetRolesAsync(await _userManager.GetUserAsync(this.User));

            var tToDelete = _database.Set<T>().FirstOrDefault(t => t.Id == id);

            if (tToDelete.OwnerId == myself || myselfRoles.Contains("Admin"))
            {
                _database.Set<T>().Remove(tToDelete);
                _database.SaveChanges();
                await _hub.Clients.All.SendAsync(tToDelete.GetType().Name + "Removed", tToDelete);
                return new JsonResult(Ok(tToDelete));
            }
            else
            {
                return new JsonResult(BadRequest());
            }
        }


        /// <summary>
        /// T módosítása, kell neki a teljes új T object a régi id-jával
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        public async Task<JsonResult> UpdateTodo([FromBody] T value)
        {
            var myself = CurrentUserId();
            var myselfRoles = await _userManager.GetRolesAsync(await _userManager.GetUserAsync(this.User));

            var tToUpdate = _database.Set<T>().FirstOrDefault(t => t.Id == value.Id);

            if (tToUpdate.OwnerId == myself || myselfRoles.Contains("Admin"))
            {
                tToUpdate.CopyFrom(value);
                _database.SaveChanges();
                await _hub.Clients.All.SendAsync(tToUpdate.GetType().Name + "Updated", tToUpdate);
                return new JsonResult(Ok());
            }
            else
            {
                return new JsonResult(BadRequest());
            }
        }





        //segédmetódus
        private string CurrentUserId()
        {
            //ki a jelenlegi user
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var myself = _userManager.Users.FirstOrDefault(t => t.UserName == userId);
            return myself.Id;
        }
    }
}