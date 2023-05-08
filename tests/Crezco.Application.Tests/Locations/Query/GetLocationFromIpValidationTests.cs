using Crezco.Application.Locations.Query;
using FluentValidation.TestHelper;

namespace Crezco.Application.Tests.Locations.Query;

public class GetLocationFromIpValidationTests
{
    [Theory]
    [InlineData("")]
    [InlineData("fake")]
    [InlineData("not.real")]
    [InlineData("abc.dwqdq.dw")]
    [InlineData("1.2.4.7.8")]
    public void InvalidIpAddress_ReturnsValidationError(string ipAddress)
    {
        var model = new GetLocationFromIp.Query(ipAddress);
        var result = new GetLocationFromIp.Validator().TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.IpAddress);
    }
}