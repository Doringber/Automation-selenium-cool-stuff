using Automation.Extensions.Contracts;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using System;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace Automation.Extensions.Components
{
    public class WebDriverFactory
    {
        private readonly DriverParams driverParams;

        public WebDriverFactory(string driverParamsJson)
            : this(LoadParams(driverParamsJson)) { }
        

        public WebDriverFactory(DriverParams driverParams)
        {
            this.driverParams = driverParams;
            if (string.IsNullOrEmpty(driverParams.Binaries)||driverParams.Binaries == ".")
            {
                driverParams.Binaries = Environment.CurrentDirectory;
            }
        }

        public string WebDriverManagerChrome()
        {
            driverParams.Binaries = new DriverManager().SetUpDriver(new ChromeConfig()).Replace("chromedriver.exe", "");
            return driverParams.Binaries;
            
        }

        public string WebDriverManagerFireFox()
        {
            driverParams.Binaries = new DriverManager().SetUpDriver(new FirefoxConfig()).Replace("geckodriver.exe", "");
            return driverParams.Binaries;
            
        }

        public IWebDriver Get()
        {
            if (!string.Equals(driverParams.Source, "REMOTE", StringComparison.OrdinalIgnoreCase))
            {
                return GetDriver();
            }
            return GertRemoteDriver();
        }

        //local webdriver
        private IWebDriver GetChrome() => new ChromeDriver(WebDriverManagerChrome());

        private IWebDriver GetFireFox() => new FirefoxDriver(WebDriverManagerFireFox());

        private IWebDriver GetDriver()
        {
            switch (driverParams.Driver)
            {
                case "CHROME": return GetChrome();
                case "FIREFOX": return GetFireFox();

                default: return GertRemoteDriver();
            }
        }

        //remote 
        private IWebDriver GetRemoteChrome() => new RemoteWebDriver(new Uri(driverParams.Binaries), new ChromeOptions());

        private IWebDriver GetRemoteFireFox() => new RemoteWebDriver(new Uri(driverParams.Binaries), new FirefoxOptions());




        private IWebDriver GertRemoteDriver()
        {
            switch (driverParams.Driver)
            {
                case "CHROME": return GetRemoteChrome();
                case "FIREFOX": return GetRemoteFireFox();

                default: return GetChrome();
            }
        }


        private static DriverParams LoadParams(string driverParamsJson)
        {
            if (string.IsNullOrEmpty(driverParamsJson))
            {
                return new DriverParams { Source = "Local", Driver = "Chrome", Binaries = "." };

            }
            return JsonConvert.DeserializeObject<DriverParams>(driverParamsJson);
        }
    }
}
