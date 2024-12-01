using Allure.Xunit.Attributes;
using Allure.Net.Commons;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Firefox;
using Xunit;
using Allure.Xunit;

public class LogoutTests : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly AllureLifecycle _allure;
    private readonly string _baseUrl = "https://localhost:7296"; // URL вашего приложения

    public LogoutTests()
    {
        var options = new FirefoxOptions();
        options.AddArgument("--start-maximized"); // Запуск браузера в полноэкранном режиме
        _driver = new FirefoxDriver(options);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        _allure = AllureLifecycle.Instance;
    }

    [Fact]
    [AllureSuite("Logout")]
    [AllureSubSuite("User Registration")]
    [AllureDescription("Проверяет успешную регистрацию пользователя")]
    [AllureOwner("Your Name")]
    public void Test_UserRegistration()
    {
        WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));

        // Шаг 1: Авторизация пользователя
        _allure.StartStep(new StepResult { name = "Авторизация пользователя" });
        _driver.Navigate().GoToUrl($"{_baseUrl}/");
        wait.Until(driver => driver.FindElement(By.Name("email"))).SendKeys("testuser@example.com");
        _driver.FindElement(By.Name("password")).SendKeys("Test@12345");
        _driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        _allure.StartStep(new StepResult { name = "Принятие alert" });
        try
        {
            var alert = wait.Until(driver => _driver.SwitchTo().Alert());
            Assert.Contains("Вы успешно вошли", alert.Text, StringComparison.OrdinalIgnoreCase);
            alert.Accept(); // Принимаем alert
        }
        catch (WebDriverTimeoutException)
        {
            Assert.False(true, "Alert с текстом 'Вы успешно вошли' не найден.");
        }
        _allure.StopStep();


        wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete")); // Ждем загрузки страницы после перенаправления
        Thread.Sleep(4000);
        Assert.Contains("/Home/Messages", _driver.Url, StringComparison.OrdinalIgnoreCase);
        _allure.StopStep();

        // Шаг 2: Выход
        _allure.StartStep(new StepResult { name = "Выход" });
        _driver.FindElement(By.XPath("//button[text()='Выйти']")).Click();
        wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete")); // Ждем загрузки страницы после перенаправления
        Thread.Sleep(4000);
        Assert.Contains("/", _driver.Url, StringComparison.OrdinalIgnoreCase);
        _allure.StopStep();
    }

    


    public void Dispose()
    {
        _driver.Quit();
    }
}
