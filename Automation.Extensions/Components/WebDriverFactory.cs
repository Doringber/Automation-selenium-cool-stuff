using Automation.Extensions.Contracts;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using System;
using OpenQA.Selenium.Edge;
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
            if (string.IsNullOrEmpty(driverParams.Binaries) || driverParams.Binaries == ".")
            {
                driverParams.Binaries = Environment.CurrentDirectory;
            }
        }

        public ChromeOptions ChromeOptionsRemote()
        {

            ChromeOptions options = new ChromeOptions();
            options.AddArguments("start-maximized"); // open Browser in maximized mode
            options.AddArguments("disable-infobars"); // disabling infobars
            options.AddArguments("--disable-extensions"); // disabling extensions
            options.AddArguments("--disable-gpu"); // applicable to windows os only
            options.AddArguments("--disable-dev-shm-usage"); // overcome limited resource problems
            options.AddArguments("--no-sandbox"); // Bypass OS security model

            return options;

        }

        public EdgeOptions EdgeOptionsRemote()
        {

            var options = new EdgeOptions();
            //options.AddAdditionalCapability("wdpAddress", "remotehost:50080");
            //options.AddAdditionalCapability("args", "remote-debugging-port=9222");
            //options.AddAdditionalCapability("ie.edgechromium", true);
            options.AddAdditionalCapability("args", "--no-sandbox");
            options.AddAdditionalCapability("args", "--disable-dev-shm-usage");
            options.AddAdditionalCapability("args", "disable-infobars");
            options.AddAdditionalCapability("args", "--disable-extensions");
            options.AddAdditionalCapability("args", "--disable-gpu");
            options.AddAdditionalCapability("args", "--disable-dev-shm-usagedisable-gpu");

            return options;

        }

        public DesiredCapabilities desiredCapabilities()
        {
            DesiredCapabilities caps = new DesiredCapabilities();
            caps.SetCapability("browser", "chrome");
            return caps;
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


        private IWebDriver GetRemoteChrome() => new RemoteWebDriver(new Uri(driverParams.Binaries), ChromeOptionsRemote().ToCapabilities(), TimeSpan.FromSeconds(600));

        private IWebDriver GetRemoteFireFox() => new RemoteWebDriver(new Uri(driverParams.Binaries), new FirefoxOptions());

        private IWebDriver GetRemoteEdge() => new RemoteWebDriver(new Uri(driverParams.Binaries), EdgeOptionsRemote().ToCapabilities(), TimeSpan.FromSeconds(600));




        private IWebDriver GertRemoteDriver()
        {
            switch (driverParams.Driver)
            {
                case "CHROME": return GetRemoteChrome();
                case "FIREFOX": return GetRemoteFireFox();
                case "EDGE": return GetRemoteEdge();

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
