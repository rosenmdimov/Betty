using Betty.Pages;
using Reqnroll;

namespace Betty.Steps;

[Binding]
public class MainPageSteps
{
    private readonly MainPage _mainPage;

    public MainPageSteps(MainPage mainPage)
    {
        _mainPage = mainPage;
    }

    [Given("I am on the Main page")]
    public async Task GivenIAmOnTheMainPage()
    {
        await _mainPage.NavigateToMainPageAsync();
    }

    [Given("I accept the cookie policy")]
    [When("I accept the cookie policy")]
    public async Task WhenIAcceptTheCookiePolicy()
    {
        await _mainPage.AcceptCookiesAsync();
    }

    [Then("The social media {string} is displayed")]
    public async Task ThenTheSocialMediaIsDisplayed(string media)
    {
        var isVisible = await _mainPage.IsMediaIconVisibleAsync(media);
        Assert.That(isVisible, Is.True, $"The '{media}' icon is not displayed.");
    }
}
