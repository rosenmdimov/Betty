using Betty.Pages;
using Microsoft.Playwright;
using Reqnroll;
using System.Threading.Tasks;

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

    [When("I accept the cookie policy")]
    public async Task WhenIAcceptTheCookiePolicy()
    {
        await _mainPage.AcceptCookiesAsync();
    }
    [Then("The page title is {string}")]
    public async Task ThenThePageTitleIs(string pageTitle)
    {
        Assert.That(await _mainPage.GetPageTitleAsync(), Is.EqualTo(pageTitle));
    }

    [Then("The social media {string} is displayed")]
    public async Task ThenTheSocialMediaIsDisplayed(string media)
    {
        var isPresent = await _mainPage.IsMediaIconVisible(media);
        Assert.That(isPresent);
    }


}