using Allure.Xunit.Attributes;
using Allure.Net.Commons;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Firefox;
using Xunit;
using Allure.Xunit;

public class MessagesTests : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly AllureLifecycle _allure;
    private readonly string _baseUrl = "https://localhost:7296"; // URL вашего приложения

    public MessagesTests()
    {
        var options = new FirefoxOptions();
        options.AddArgument("--start-maximized"); // Запуск браузера в полноэкранном режиме
        _driver = new FirefoxDriver(options);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        _allure = AllureLifecycle.Instance;
    }

    [Fact]
    [AllureSuite("Messages")]
    [AllureSubSuite("Create chat")]
    [AllureDescription("Проверяет успешное создание чата")]
    public void Test_CreateChat()
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


        // Шаг 2: Создание чата
        _allure.StartStep(new StepResult { name = "Создание чата" });
        wait.Until(driver => driver.FindElement(By.Id("userSelect"))).Click();
        var userSelect = new SelectElement(_driver.FindElement(By.Id("userSelect")));
        userSelect.SelectByIndex(18); // Выбираем первого пользователя
        _driver.FindElement(By.XPath("//button[text()='Создать чат']")).Click();
        _allure.StopStep();

        _allure.StartStep(new StepResult { name = "Проверка сообщения 'Чат создан успешно!'" });
        try
        {
            wait.Until(driver =>
            {
                var alert = _driver.SwitchTo().Alert();
                var alertText = alert.Text;
                Assert.Contains("Чат создан успешно!", alertText);
                alert.Accept();
                return true;
            });
        }
        catch (WebDriverTimeoutException)
        {
            Assert.False(true, "Alert с текстом 'Чат создан успешно!' не найден.");
        }
    }

    [Fact]
    [AllureSuite("Messages")]
    [AllureSubSuite("Create chat user null")]
    [AllureDescription("Проверяет ошибку в создании чата не выбрав пользователя")]
    public void Test_CreateChatUserNull()
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


        // Шаг 2: Создание чата
        _allure.StartStep(new StepResult { name = "Создание чата" });
        _driver.FindElement(By.XPath("//button[text()='Создать чат']")).Click();
        _allure.StopStep();

        _allure.StartStep(new StepResult { name = "Проверка сообщения 'Выберете пользователя с которым хотите начать чат'" });
        try
        {
            wait.Until(driver =>
            {
                var alert = _driver.SwitchTo().Alert();
                var alertText = alert.Text;
                Assert.Contains("Выберете пользователя с которым хотите начать чат", alertText);
                alert.Accept();
                return true;
            });
        }
        catch (WebDriverTimeoutException)
        {
            Assert.False(true, "Alert с текстом 'Выберете пользователя с которым хотите начать чат' не найден.");
        }
    }

    [Fact]
    [AllureSuite("Messages")]
    [AllureSubSuite("Create chat already create")]
    [AllureDescription("Проверяет ошибку в создании уже существуещего чата")]
    public void Test_CreateChatAlredyCreate()
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


        // Шаг 2: Создание чата
        _allure.StartStep(new StepResult { name = "Создание чата" });
        wait.Until(driver => driver.FindElement(By.Id("userSelect"))).Click();
        var userSelect = new SelectElement(_driver.FindElement(By.Id("userSelect")));
        userSelect.SelectByIndex(1); // Выбираем первого пользователя
        _driver.FindElement(By.XPath("//button[text()='Создать чат']")).Click();
        _allure.StopStep();

        _allure.StartStep(new StepResult { name = "Проверка сообщения 'Чат уже существует'" });
        try
        {
            wait.Until(driver =>
            {
                var alert = _driver.SwitchTo().Alert();
                var alertText = alert.Text;
                Assert.Contains("Чат уже существует", alertText);
                alert.Accept();
                return true;
            });
        }
        catch (WebDriverTimeoutException)
        {
            Assert.False(true, "Alert с текстом 'Чат уже существует' не найден.");
        }
    }

    [Fact]
    [AllureSuite("Messages")]
    [AllureSubSuite("Send Message")]
    [AllureDescription("Проверяет успешную отправку сообщения")]
    public void Test_SendMessage()
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

        // Шаг 3: Выбор чата

        _allure.StartStep(new StepResult { name = "Выбор чата" });
        wait.Until(driver => driver.FindElement(By.Id("userSelect"))).Click();
        var userSelect = new SelectElement(_driver.FindElement(By.Id("userSelect")));
        userSelect.SelectByIndex(6); // Выбираем первого пользователя
        wait.Until(driver => driver.FindElement(By.Id("chatSelect"))).Click();
        var chatSelect = new SelectElement(_driver.FindElement(By.Id("chatSelect")));
        chatSelect.SelectByIndex(6); // Выбираем первый доступный чат
        _allure.StopStep();

        // Шаг 4: Отправка сообщения
        _allure.StartStep(new StepResult { name = "Отправка сообщения" });
        var messageInput = _driver.FindElement(By.Id("messageContent"));
        messageInput.SendKeys("Hello, this is a test message!");
        _driver.FindElement(By.XPath("//button[text()='Отправить сообщение']")).Click();
        _allure.StopStep();
        Thread.Sleep(4000);
        _allure.StartStep(new StepResult { name = "Проверка сообщения 'Сообщение отправлено успешно!'" });
        try
        {
            wait.Until(driver =>
            {
                var alert = _driver.SwitchTo().Alert();
                var alertText = alert.Text;
                Assert.Contains("Сообщение отправлено успешно!", alertText);
                alert.Accept();
                return true;
            });
        }
        catch (WebDriverTimeoutException)
        {
            Assert.False(true, "Alert с текстом 'Сообщение отправлено успешно!' не найден.");
        }
    }

    [Fact]
    [AllureSuite("Messages")]
    [AllureSubSuite("Send Message no chat")]
    [AllureDescription("Проверяет ошибку в отвравке сообщения не выбрав чат")]
    public void Test_SendMessageNoChat()
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


        // Шаг 4: Отправка сообщения
        _allure.StartStep(new StepResult { name = "Отправка сообщения" });
        var messageInput = _driver.FindElement(By.Id("messageContent"));
        messageInput.SendKeys("Hello, this is a test message!");
        _driver.FindElement(By.XPath("//button[text()='Отправить сообщение']")).Click();
        _allure.StopStep();

        _allure.StartStep(new StepResult { name = "Проверка сообщения 'Сначала выберите чат'" });
        try
        {
            wait.Until(driver =>
            {
                var alert = _driver.SwitchTo().Alert();
                var alertText = alert.Text;
                Assert.Contains("Сначала выберите чат", alertText);
                alert.Accept();
                return true;
            });
        }
        catch (WebDriverTimeoutException)
        {
            Assert.False(true, "Alert с текстом 'Сначала выберите чат' не найден.");
        }
    }


    public void Dispose()
    {
        _driver.Quit();
    }
}
