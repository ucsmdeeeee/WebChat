using Allure.Xunit.Attributes;
using Allure.Net.Commons;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Firefox;
using Xunit;
using Allure.Xunit;

public class LoginTests : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly AllureLifecycle _allure;
    private readonly string _baseUrl = "https://localhost:7296"; // URL вашего приложения

    public LoginTests()
    {
        var options = new FirefoxOptions();
        options.AddArgument("--start-maximized"); // Запуск браузера в полноэкранном режиме
        _driver = new FirefoxDriver(options);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        _allure = AllureLifecycle.Instance;
    }

    [Fact]
    [AllureSuite("Login")]
    [AllureSubSuite("User Login")]
    [AllureDescription("Проверяет успешный вход в систему после регистрации")]
    public void Test_UserLogin()
    {
        WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));

        // Шаг 1: Открытие страницы входа
        _allure.StartStep(new StepResult { name = "Открытие страницы входа" });
        _driver.Navigate().GoToUrl($"{_baseUrl}/");
        wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        _allure.StopStep();

        // Шаг 2: Ввод учетных данных
        _allure.StartStep(new StepResult { name = "Ввод учетных данных" });
        wait.Until(driver => driver.FindElement(By.Name("email"))).SendKeys("testuser@example.com");
        _driver.FindElement(By.Name("password")).SendKeys("Test@12345");
        _allure.StopStep();

        // Шаг 3: Нажатие кнопки входа
        _allure.StartStep(new StepResult { name = "Нажатие кнопки входа" });
        wait.Until(driver => driver.FindElement(By.CssSelector("button[type='submit']"))).Click();
        _allure.StopStep();

        // Шаг 4: Проверка успешного входа
        _allure.StartStep(new StepResult { name = "Проверка сообщения 'Вы успешно вошли'" });
        try
        {
            wait.Until(driver =>
            {
                var alert = _driver.SwitchTo().Alert();
                var alertText = alert.Text;
                Assert.Contains("Вы успешно вошли", alertText);
                alert.Accept();
                return true;
            });
        }
        catch (WebDriverTimeoutException)
        {
            Assert.False(true, "Alert с текстом 'Вы успешно вошли' не найден.");
        }
        _allure.StopStep();
    }

    [Fact]
    [AllureSuite("Login")]
    [AllureSubSuite("User Login false email")]
    [AllureDescription("Проверяет успешный вход в систему после регистрации")]
    public void Test_UserLoginFalseEmail()
    {
        WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));

        // Шаг 1: Открытие страницы входа
        _allure.StartStep(new StepResult { name = "Открытие страницы входа" });
        _driver.Navigate().GoToUrl($"{_baseUrl}/");
        wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        _allure.StopStep();

        // Шаг 2: Ввод учетных данных
        _allure.StartStep(new StepResult { name = "Ввод учетных данных" });
        wait.Until(driver => driver.FindElement(By.Name("email"))).SendKeys("falseemail@example.com");
        _driver.FindElement(By.Name("password")).SendKeys("Test@12345");
        _allure.StopStep();

        // Шаг 3: Нажатие кнопки входа
        _allure.StartStep(new StepResult { name = "Нажатие кнопки входа" });
        wait.Until(driver => driver.FindElement(By.CssSelector("button[type='submit']"))).Click();
        _allure.StopStep();

        // Шаг 4: Проверка успешного входа
        _allure.StartStep(new StepResult { name = "Проверка сообщения 'Неправильный email или пароль'" });
        try
        {
            wait.Until(driver =>
            {
                var alert = _driver.SwitchTo().Alert();
                var alertText = alert.Text;
                Assert.Contains("Неправильный email или пароль", alertText);
                alert.Accept();
                return true;
            });
        }
        catch (WebDriverTimeoutException)
        {
            Assert.False(true, "Alert с текстом 'Неправильный email или пароль' не найден.");
        }
        _allure.StopStep();
    }

    [Fact]
    [AllureSuite("Login")]
    [AllureSubSuite("User Login false password")]
    [AllureDescription("Проверяет успешный вход в систему после регистрации")]
    public void Test_UserLoginFalsePassword()
    {
        WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));

        // Шаг 1: Открытие страницы входа
        _allure.StartStep(new StepResult { name = "Открытие страницы входа" });
        _driver.Navigate().GoToUrl($"{_baseUrl}/");
        wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        _allure.StopStep();

        // Шаг 2: Ввод учетных данных
        _allure.StartStep(new StepResult { name = "Ввод учетных данных" });
        wait.Until(driver => driver.FindElement(By.Name("email"))).SendKeys("testuser@example.com");
        _driver.FindElement(By.Name("password")).SendKeys("falsepassword");
        _allure.StopStep();

        // Шаг 3: Нажатие кнопки входа
        _allure.StartStep(new StepResult { name = "Нажатие кнопки входа" });
        wait.Until(driver => driver.FindElement(By.CssSelector("button[type='submit']"))).Click();
        _allure.StopStep();

        // Шаг 4: Проверка успешного входа
        _allure.StartStep(new StepResult { name = "Проверка сообщения 'Неправильный email или пароль'" });
        try
        {
            wait.Until(driver =>
            {
                var alert = _driver.SwitchTo().Alert();
                var alertText = alert.Text;
                Assert.Contains("Неправильный email или пароль", alertText);
                alert.Accept();
                return true;
            });
        }
        catch (WebDriverTimeoutException)
        {
            Assert.False(true, "Alert с текстом 'Неправильный email или пароль' не найден.");
        }
        _allure.StopStep();
    }

    public void Dispose()
    {
        _driver.Quit();
    }
}
