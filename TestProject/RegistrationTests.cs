using Allure.Xunit.Attributes;
using Allure.Net.Commons;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Firefox;
using Xunit;
using Allure.Xunit;

public class RegistrationTests : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly AllureLifecycle _allure;
    private readonly string _baseUrl = "https://localhost:7296"; // URL вашего приложения

    public RegistrationTests()
    {
        var options = new FirefoxOptions();
        options.AddArgument("--start-maximized"); // Запуск браузера в полноэкранном режиме
        _driver = new FirefoxDriver(options);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        _allure = AllureLifecycle.Instance;
    }

    [Fact]
    [AllureSuite("Registration")]
    [AllureSubSuite("User Registration")]
    [AllureDescription("Проверяет успешную регистрацию пользователя")]
    [AllureOwner("Your Name")]
    public void Test_UserRegistration()
    {
        WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));

        // Шаг 1: Открытие страницы регистрации
        _allure.StartStep(new StepResult { name = "Открытие страницы регистрации" });
        _driver.Navigate().GoToUrl($"{_baseUrl}/home/register");
        wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        _allure.StopStep();

        // Шаг 2: Заполнение формы регистрации
        _allure.StartStep(new StepResult { name = "Заполнение формы регистрации" });
        wait.Until(driver => driver.FindElement(By.Name("username"))).SendKeys("testuser11111111111");
        _driver.FindElement(By.Name("email")).SendKeys("testuser46@example.com");
        _driver.FindElement(By.Name("password")).SendKeys("Test@12345");
        _driver.FindElement(By.Name("confirmPassword")).SendKeys("Test@12345");
        _allure.StopStep();

        // Шаг 3: Нажатие кнопки регистрации
        _allure.StartStep(new StepResult { name = "Нажатие кнопки регистрации" });
        wait.Until(driver => driver.FindElement(By.CssSelector("button[type='submit']"))).Click();
        _allure.StopStep();

        // Шаг 4: Проверка успешной регистрации
        _allure.StartStep(new StepResult { name = "Проверка успешности регистрации" });
        try
        {
            wait.Until(driver =>
            {
                var alert = _driver.SwitchTo().Alert();
                var alertText = alert.Text;
                Assert.Contains("Вы успешно зарегистрировались", alertText);
                alert.Accept();
                return true;
            });
        }
        catch (WebDriverTimeoutException)
        {
            Assert.False(true, "Alert с текстом 'Вы успешно зарегистрировались' не найден.");
        }
        _allure.StopStep();
    }

    [Fact]
    [AllureSuite("Registration")]
    [AllureSubSuite("User Registration email already use")]
    [AllureDescription("Проверяет успешную регистрацию пользователя")]
    [AllureOwner("Your Name")]
    public void Test_UserRegistrationEmailAlreadyUse()
    {
        WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));

        // Шаг 1: Открытие страницы регистрации
        _allure.StartStep(new StepResult { name = "Открытие страницы регистрации" });
        _driver.Navigate().GoToUrl($"{_baseUrl}/home/register");
        wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        _allure.StopStep();

        // Шаг 2: Заполнение формы регистрации
        _allure.StartStep(new StepResult { name = "Заполнение формы регистрации" });
        wait.Until(driver => driver.FindElement(By.Name("username"))).SendKeys("testuser");
        _driver.FindElement(By.Name("email")).SendKeys("testuser@example.com");
        _driver.FindElement(By.Name("password")).SendKeys("Test@12345");
        _driver.FindElement(By.Name("confirmPassword")).SendKeys("Test@12345");
        _allure.StopStep();

        // Шаг 3: Нажатие кнопки регистрации
        _allure.StartStep(new StepResult { name = "Нажатие кнопки регистрации" });
        wait.Until(driver => driver.FindElement(By.CssSelector("button[type='submit']"))).Click();
        _allure.StopStep();

        // Шаг 4: Проверка успешной регистрации
        _allure.StartStep(new StepResult { name = "Проверка успешности регистрации" });
        try
        {
            wait.Until(driver =>
            {
                var alert = _driver.SwitchTo().Alert();
                var alertText = alert.Text;
                Assert.Contains("Пользователь с данным email уже зарегестрирован", alertText);
                alert.Accept();
                return true;
            });
        }
        catch (WebDriverTimeoutException)
        {
            Assert.False(true, "Alert с текстом 'Пользователь с данным email уже зарегестрирован' не найден.");
        }
        _allure.StopStep();
    }


    [Fact]
    [AllureSuite("Registration")]
    [AllureSubSuite("User Registration invalid confirm password")]
    [AllureDescription("Проверяет успешную регистрацию пользователя")]
    [AllureOwner("Your Name")]
    public void Test_UserRegistrationInvalidConfirmPassword()
    {
        WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));

        // Шаг 1: Открытие страницы регистрации
        _allure.StartStep(new StepResult { name = "Открытие страницы регистрации" });
        _driver.Navigate().GoToUrl($"{_baseUrl}/home/register");
        wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        _allure.StopStep();

        // Шаг 2: Заполнение формы регистрации
        _allure.StartStep(new StepResult { name = "Заполнение формы регистрации" });
        wait.Until(driver => driver.FindElement(By.Name("username"))).SendKeys("testuser");
        _driver.FindElement(By.Name("email")).SendKeys("testuser123@example.com");
        _driver.FindElement(By.Name("password")).SendKeys("1");
        _driver.FindElement(By.Name("confirmPassword")).SendKeys("12");
        _allure.StopStep();

        // Шаг 3: Нажатие кнопки регистрации
        _allure.StartStep(new StepResult { name = "Нажатие кнопки регистрации" });
        wait.Until(driver => driver.FindElement(By.CssSelector("button[type='submit']"))).Click();
        _allure.StopStep();
        _allure.StartStep(new StepResult { name = "Проверка успешности регистрации" });
        try
        {
            wait.Until(driver =>
            {
                var alert = _driver.SwitchTo().Alert();
                var alertText = alert.Text;
                Assert.Contains("Пароли различаются", alertText);
                alert.Accept();
                return true;
            });
        }
        catch (WebDriverTimeoutException)
        {
            Assert.False(true, "Alert с текстом 'Пароли различаются' не найден.");
        }
        _allure.StopStep();
    }


    public void Dispose()
    {
        _driver.Quit();
    }
}
