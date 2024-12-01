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
    private readonly string _baseUrl = "https://localhost:7296"; // URL ������ ����������

    public RegistrationTests()
    {
        var options = new FirefoxOptions();
        options.AddArgument("--start-maximized"); // ������ �������� � ������������� ������
        _driver = new FirefoxDriver(options);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        _allure = AllureLifecycle.Instance;
    }

    [Fact]
    [AllureSuite("Registration")]
    [AllureSubSuite("User Registration")]
    [AllureDescription("��������� �������� ����������� ������������")]
    [AllureOwner("Your Name")]
    public void Test_UserRegistration()
    {
        WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));

        // ��� 1: �������� �������� �����������
        _allure.StartStep(new StepResult { name = "�������� �������� �����������" });
        _driver.Navigate().GoToUrl($"{_baseUrl}/home/register");
        wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        _allure.StopStep();

        // ��� 2: ���������� ����� �����������
        _allure.StartStep(new StepResult { name = "���������� ����� �����������" });
        wait.Until(driver => driver.FindElement(By.Name("username"))).SendKeys("testuser11111111111");
        _driver.FindElement(By.Name("email")).SendKeys("testuser46@example.com");
        _driver.FindElement(By.Name("password")).SendKeys("Test@12345");
        _driver.FindElement(By.Name("confirmPassword")).SendKeys("Test@12345");
        _allure.StopStep();

        // ��� 3: ������� ������ �����������
        _allure.StartStep(new StepResult { name = "������� ������ �����������" });
        wait.Until(driver => driver.FindElement(By.CssSelector("button[type='submit']"))).Click();
        _allure.StopStep();

        // ��� 4: �������� �������� �����������
        _allure.StartStep(new StepResult { name = "�������� ���������� �����������" });
        try
        {
            wait.Until(driver =>
            {
                var alert = _driver.SwitchTo().Alert();
                var alertText = alert.Text;
                Assert.Contains("�� ������� ������������������", alertText);
                alert.Accept();
                return true;
            });
        }
        catch (WebDriverTimeoutException)
        {
            Assert.False(true, "Alert � ������� '�� ������� ������������������' �� ������.");
        }
        _allure.StopStep();
    }

    [Fact]
    [AllureSuite("Registration")]
    [AllureSubSuite("User Registration email already use")]
    [AllureDescription("��������� �������� ����������� ������������")]
    [AllureOwner("Your Name")]
    public void Test_UserRegistrationEmailAlreadyUse()
    {
        WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));

        // ��� 1: �������� �������� �����������
        _allure.StartStep(new StepResult { name = "�������� �������� �����������" });
        _driver.Navigate().GoToUrl($"{_baseUrl}/home/register");
        wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        _allure.StopStep();

        // ��� 2: ���������� ����� �����������
        _allure.StartStep(new StepResult { name = "���������� ����� �����������" });
        wait.Until(driver => driver.FindElement(By.Name("username"))).SendKeys("testuser");
        _driver.FindElement(By.Name("email")).SendKeys("testuser@example.com");
        _driver.FindElement(By.Name("password")).SendKeys("Test@12345");
        _driver.FindElement(By.Name("confirmPassword")).SendKeys("Test@12345");
        _allure.StopStep();

        // ��� 3: ������� ������ �����������
        _allure.StartStep(new StepResult { name = "������� ������ �����������" });
        wait.Until(driver => driver.FindElement(By.CssSelector("button[type='submit']"))).Click();
        _allure.StopStep();

        // ��� 4: �������� �������� �����������
        _allure.StartStep(new StepResult { name = "�������� ���������� �����������" });
        try
        {
            wait.Until(driver =>
            {
                var alert = _driver.SwitchTo().Alert();
                var alertText = alert.Text;
                Assert.Contains("������������ � ������ email ��� ���������������", alertText);
                alert.Accept();
                return true;
            });
        }
        catch (WebDriverTimeoutException)
        {
            Assert.False(true, "Alert � ������� '������������ � ������ email ��� ���������������' �� ������.");
        }
        _allure.StopStep();
    }


    [Fact]
    [AllureSuite("Registration")]
    [AllureSubSuite("User Registration invalid confirm password")]
    [AllureDescription("��������� �������� ����������� ������������")]
    [AllureOwner("Your Name")]
    public void Test_UserRegistrationInvalidConfirmPassword()
    {
        WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));

        // ��� 1: �������� �������� �����������
        _allure.StartStep(new StepResult { name = "�������� �������� �����������" });
        _driver.Navigate().GoToUrl($"{_baseUrl}/home/register");
        wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        _allure.StopStep();

        // ��� 2: ���������� ����� �����������
        _allure.StartStep(new StepResult { name = "���������� ����� �����������" });
        wait.Until(driver => driver.FindElement(By.Name("username"))).SendKeys("testuser");
        _driver.FindElement(By.Name("email")).SendKeys("testuser123@example.com");
        _driver.FindElement(By.Name("password")).SendKeys("1");
        _driver.FindElement(By.Name("confirmPassword")).SendKeys("12");
        _allure.StopStep();

        // ��� 3: ������� ������ �����������
        _allure.StartStep(new StepResult { name = "������� ������ �����������" });
        wait.Until(driver => driver.FindElement(By.CssSelector("button[type='submit']"))).Click();
        _allure.StopStep();
        _allure.StartStep(new StepResult { name = "�������� ���������� �����������" });
        try
        {
            wait.Until(driver =>
            {
                var alert = _driver.SwitchTo().Alert();
                var alertText = alert.Text;
                Assert.Contains("������ �����������", alertText);
                alert.Accept();
                return true;
            });
        }
        catch (WebDriverTimeoutException)
        {
            Assert.False(true, "Alert � ������� '������ �����������' �� ������.");
        }
        _allure.StopStep();
    }


    public void Dispose()
    {
        _driver.Quit();
    }
}
