﻿using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiteImprove.Optimizely.Plugin.Helper;
using SiteImprove.Optimizely.Plugin.Models;
using SiteImprove.Optimizely.Plugin.Repositories;

namespace SiteImprove.Optimizely.Plugin.Controllers
{
    [Authorize(Policy = Constants.SiteImproveAuthorizationPolicy)]
    public class SiteimproveAdminController : Controller
    {
        private readonly ISettingsRepository _settingsRepo;
        private readonly ISiteimproveHelper _siteimproveHelper;

        public SiteimproveAdminController(ISettingsRepository settingsRepo, ISiteimproveHelper siteimproveHelper)
        {
            _settingsRepo = settingsRepo;
            _siteimproveHelper = siteimproveHelper;
            Title = "SiteImprove";
        }

        [ViewData]
        public string Title { get; set; }

        public ActionResult Index(bool newToken = false, bool prepublishError = false)
        {
            var settings = _settingsRepo.GetSetting();
            if (newToken)
            {
                settings.Token = _siteimproveHelper.RequestToken();
                _settingsRepo.SaveToken(settings.Token, settings.Recheck, settings.ApiUser, settings.ApiKey);
            }

            var vm = new SettingsViewModel()
            {
                Token = settings.Token,
                Recheck = settings.Recheck,
                ApiUser = settings.ApiUser,
                ApiKey = settings.ApiKey,
                PrepublishCheckEnabled = _siteimproveHelper.GetPrepublishCheckEnabled(settings.ApiUser, settings.ApiKey),
                PrepublishError = prepublishError,
                UrlMap = settings.UrlMap
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult Save(bool recheck, string apiUser, string apiKey, IEnumerable<KeyValuePair<string, string>> urlMap)
        {
            var settings = this._settingsRepo.GetSetting();
            settings.Recheck = recheck;
            settings.ApiUser = apiUser;
            settings.ApiKey = apiKey;

            settings.UrlMap = new Dictionary<string, string>();
            foreach (var pair in urlMap)
            {
                if (Uri.TryCreate(pair.Key, UriKind.Absolute, out Uri _) &&
                    Uri.TryCreate(pair.Value, UriKind.Absolute, out Uri _))
                {
                    settings.UrlMap.TryAdd(pair.Key, pair.Value);
                }
            }

            _settingsRepo.SaveToken(settings.Token, settings.Recheck, settings.ApiUser, settings.ApiKey, settings.UrlMap);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult EnablePrepublishCheck(bool enablePrepublishCheck = false)
        {
            var settings = _settingsRepo.GetSetting();
            var success = true;

            if (enablePrepublishCheck)
            {
                success = _siteimproveHelper.EnablePrepublishCheck(settings.ApiUser, settings.ApiKey);
            }

            if (!success)
            {
                return RedirectToAction("Index", new { prepublishError = true });
            }

            return RedirectToAction("Index");
        }
    }
}
