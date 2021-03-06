﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoMapper;
using DataAccess.Repositories.Interfaces;
using Domain.Response;
using Domain.Services.Interfaces;
using Model.ControllerModel;
using Model.DomainModel;

namespace Domain.Services
{
    public class RestaurantManagerService : IRestaurantManagerService
    {
        #region Fields

        private readonly IRestaurantManagerRepository _restaurantManagerRepository;
        private readonly IAccountService _accountService;
        private readonly IRestaurantService _restaurantService;

        #endregion

        #region Constructor

        public RestaurantManagerService(IRestaurantManagerRepository restaurantManagerRepository, IAccountService accountService, IRestaurantService restaurantService)
        {
            if (restaurantManagerRepository == null) throw new ArgumentNullException("restaurantManagerRepository");
            if (accountService == null) throw new ArgumentNullException("accountService");
            if (restaurantService == null) throw new ArgumentNullException("restaurantService");

            _restaurantManagerRepository = restaurantManagerRepository;
            _accountService = accountService;
            _restaurantService = restaurantService;
        }

        #endregion

        #region Methods

        public IResponse Create(RestaurantManagerWithAccount restaurantManagerWithAccount)
        {
            var response = new Response.Response();

            if (_accountService.IsUsernameAlreadyTaken(restaurantManagerWithAccount.Account.Username))
            {
                response.Set(HttpStatusCode.BadRequest, "Username already exist");
                return response;
            }

            var restaurantManager = Mapper.Map<RestaurantManagerWithAccount, RestaurantManager>(restaurantManagerWithAccount);

            restaurantManagerWithAccount.Account.AccountType = "Restaurant Manager";
            _accountService.CreateAccount(restaurantManagerWithAccount.Account);
            restaurantManager.Username = restaurantManagerWithAccount.Account.Username;

            RemoveInexistingRestaurantIds(restaurantManager);

            _restaurantManagerRepository.Insert(restaurantManager);

            response.Set(HttpStatusCode.Created);
            return response;
        }

        private void RemoveInexistingRestaurantIds(RestaurantManager restaurantManager)
        {
            var idsToRemove = restaurantManager.RestaurantIds.Where(restaurantId => _restaurantService.GetRestaurantById(restaurantId).HttpStatusCode == HttpStatusCode.NotFound).ToList();

            foreach (var idToRemove in idsToRemove)
            {
                restaurantManager.RestaurantIds.Remove(idToRemove);
            }
        }

        public IResponse Update(RestaurantManager restaurantManager)
        {
            var response = new Response.Response();

            var existingRestaurantManager = _restaurantManagerRepository.GetSingle(r => r.Username == restaurantManager.Username);
            if (existingRestaurantManager == null)
            {
                response.Set(HttpStatusCode.NotFound, "No restaurant manager found");
                return response;
            }

            RemoveInexistingRestaurantIds(restaurantManager);

            _restaurantManagerRepository.Save(restaurantManager);

            response.Set(HttpStatusCode.NoContent);
            return response;
        }

        public IResponse Delete(string username)
        {
            var response = new Response.Response();

            _restaurantManagerRepository.Delete(username);
            _accountService.Delete(username);

            response.Set(HttpStatusCode.OK);
            return response;
        }

        public IResponse GetRestaurantManagerByUsername(string username)
        {
            var response = new Response.Response();
            var restaurantManager = _restaurantManagerRepository.GetSingle(r => r.Username == username);

            if (restaurantManager == null)
            {
                response.Set(HttpStatusCode.NotFound, "No restaurant manager found");
                return response;
            }

            response.Set(HttpStatusCode.OK, restaurantManager);
            return response;
        }

        public IResponse GetRestaurantManagerByContractorUsername(string contractorUsername)
        {
            var response = new Response.Response();

            var restaurantManagers = _restaurantManagerRepository.GetAll();

            if (restaurantManagers == null)
            {
                response.Set(HttpStatusCode.NotFound, "No restaurant manager found");
                return response;
            }

            response.Set(HttpStatusCode.OK, restaurantManagers.Where(rm => rm.ContractorUsername == contractorUsername).ToList());
            return response;
        }

        public IResponse DeleteRestaurant(string restaurantId)
        {
            var response = new Response.Response();

            var restaurantManagers = _restaurantManagerRepository.GetAll();

            foreach (var restaurantManager in restaurantManagers.Where(r => r.RestaurantIds.Contains(restaurantId)))
            {
                restaurantManager.RestaurantIds.Remove(restaurantId);
                Update(restaurantManager);
            }

            _restaurantService.Delete(restaurantId);

            response.Set(HttpStatusCode.OK);
            return response;
        }

        public IResponse GetAll()
        {
            var response = new Response.Response();

            var restaurantManagers = _restaurantManagerRepository.GetAll();

            if (!restaurantManagers.Any())
            {
                response.Set(HttpStatusCode.NoContent, "No restaurant managers found");
                return response;
            }

            var restaurantManagersWithRestaurant = new List<RestaurantManagerWithRestaurants>();

            foreach (var restaurantManager in restaurantManagers)
            {
                var restaurantManagerWithRestaurant = Mapper.Map<RestaurantManager, RestaurantManagerWithRestaurants>(restaurantManager);
                restaurantManagerWithRestaurant.Restaurants = _restaurantService.GetAllRestaurantsByRestaurantIds(restaurantManager.RestaurantIds);
                restaurantManagersWithRestaurant.Add(restaurantManagerWithRestaurant);
            }

            response.Set(HttpStatusCode.OK, restaurantManagersWithRestaurant);

            return response;
        }

        #endregion
    }
}