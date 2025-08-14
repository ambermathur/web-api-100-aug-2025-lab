using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Shows.Api.Api.Business;

public class Status(TimeProvider clock) : ControllerBase
{
    [HttpGet("/api/status")]
    public async Task<ActionResult> GetTheStatus()
    {
        if (WeAreCurrentlyOpen())
        {
            return Ok(new StatusResponse("All Good", true));
        }
        else
        {
            return Ok(new StatusResponse("All Good", false));
        }

    }

    private  bool WeAreCurrentlyOpen()
    {
        return clock.GetLocalNow().Hour > 9 && clock.GetLocalNow().Hour < 17;
    }
}

public record StatusResponse(string Message, bool IsOpen);
