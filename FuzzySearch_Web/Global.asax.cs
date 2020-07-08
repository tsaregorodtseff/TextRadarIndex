using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.Entity;
using FuzzySearchDemo.Models;

namespace FuzzySearchDemo
{
    public class MvcApplication : System.Web.HttpApplication
    {


        public string[] Pages_Index_WarAndPeace;
        public int NumberOfPages_Index_WarAndPeace;
        public Models.SearchIndex.WordsIndexItem[] SearchWordsIndex_WarAndPeace;

        protected void Application_Start()
        {

            Pages_Index_WarAndPeace = Models.Search_Index_WarAndPeace.СreatePages();
            NumberOfPages_Index_WarAndPeace = Pages_Index_WarAndPeace.Length;
            SearchWordsIndex_WarAndPeace = Models.Search_Index_WarAndPeace.CreateWordsIndex();

            Application["Pages_Index_WarAndPeace"] = Pages_Index_WarAndPeace;
            Application["NumberOfPages_Index_WarAndPeace"] = NumberOfPages_Index_WarAndPeace;
            Application["SearchWordsIndex_WarAndPeace"] = SearchWordsIndex_WarAndPeace;

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

        }
    }
}
