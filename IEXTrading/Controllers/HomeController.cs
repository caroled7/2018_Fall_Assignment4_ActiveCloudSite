using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IEXTrading.Infrastructure.IEXTradingHandler;
using IEXTrading.Models;
using IEXTrading.DataAccess;
using Newtonsoft.Json;

namespace MVCTemplate.Controllers
{
    public class HomeController : Controller
    {
        public ApplicationDbContext dbContext;

        public HomeController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        /****
         * The Symbols action calls the GetSymbols method that returns a list of Companies.
         * This list of Companies is passed to the Symbols View.
       
         public IActionResult Symbols()
        {
            //Set ViewBag variable first
            ViewBag.dbSucessComp = 0;
            IEXHandler webHandler = new IEXHandler();
            List<Company> companies = webHandler.GetSymbols();

            //Save comapnies in TempData
            TempData["Companies"] = JsonConvert.SerializeObject(companies);

            return View(companies);
        }
         ****/

        /****
 * The Symbols action calls the GetSymbols method that returns a list of Companies.
 * This list of Companies is passed to the Symbols View.
****/
        public IActionResult Symbols() /*test new -- I want to add table count --*/
        {
            int startrange = 0;
            int rangecount = 10;
            int tablecount = dbContext.Companies.Count();
            if (tablecount != 0 )
            {
                startrange = tablecount + 1;

            }

            //Set ViewBag variable first
            ViewBag.dbSucessComp = 0;
            IEXHandler webHandler = new IEXHandler();
            List<Company> companies = webHandler.GetSymbols(startrange, rangecount);

            //Save comapnies in TempData
            TempData["Companies"] = JsonConvert.SerializeObject(companies);

            return View(companies);

        




        }









        /****
         * The Refresh action calls the ClearTables method to delete records from a or all tables.
         * Count of current records for each table is passed to the Refresh View.
        ****/
        public IActionResult Refresh(string tableToDel)
        {
            ClearTables(tableToDel);
            Dictionary<string, int> tableCount = new Dictionary<string, int>();
            tableCount.Add("Companies", dbContext.Companies.Count());
            return View(tableCount);
        }

        /****
         * Saves the Symbols in database.
        ****/
        public IActionResult PopulateSymbols()
        {
            List<Company> companies = JsonConvert.DeserializeObject<List<Company>>(TempData["Companies"].ToString());
            foreach (Company company in companies)
            {
                //Database will give PK constraint violation error when trying to insert record with existing PK.
                //So add company only if it doesnt exist, check existence using symbol (PK)
                if (dbContext.Companies.Where(c => c.symbol.Equals(company.symbol)).Count() == 0)
                {
                    dbContext.Companies.Add(company);
                }
            }
            dbContext.SaveChanges();
            ViewBag.dbSuccessComp = 1;
            return View("Symbols", companies);
        }

       
        /****
         * Deletes the records from tables.
        ****/
        public void ClearTables(string tableToDel)
        {
            if ("all".Equals(tableToDel))
            {
                //First remove equities and then the companies
                dbContext.Companies.RemoveRange(dbContext.Companies);
            }
            else if ("Companies".Equals(tableToDel))
            {
                //Remove only those that don't have Equity stored in the Equitites table
                dbContext.Companies.RemoveRange(dbContext.Companies  );
            }

            dbContext.SaveChanges();
        }

        public IActionResult StockPickings()
            {
                Company companyRead1 = dbContext.Companies
                .Include(c => c.symbol)
                .Where(c => (c.close-c.week52Low)/(c.week52high-c.week52Low) > 0.82f)
                .OrderByDescending(c => c)
                .First();

                Company companyRead2 = dbContext.Companies
                .Include(c => c.symbol)
                .Where(c => (c.close-c.week52Low)/(c.week52high-c.week52Low) < 0.41f)
                .OrderBy(c => c)
                .First();

            string Buy1 = companyRead1[0];
            string Buy2 = companyRead1[1];
            string Buy3 = companyRead1[2];
            string Sell1 = companyRead2[0];
            string Sell2 = companyRead2[1];
            string Sell3 = companyRead2[2];

            return View();

            }
 



    }
}
