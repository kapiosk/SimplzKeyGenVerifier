using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimplzKeyGenVerifier.Data;
using SimplzKeyGenVerifier.Services;

namespace SimplzKeyGenVerifier
{
    public class Endpoints
    {
        public static IResult Ping([FromServices] JWTService jwt)
        {
            Dictionary<string, object> dict = new();
            dict.Add("Ping", DateTime.UtcNow);
            return Results.Text(jwt.WriteToken(dict));
        }

        public static async Task<IResult> RequestKeyAsync(
            [FromServices] JWTService jwt,
            [FromServices] AppDbContext context,
            [FromBody] string token,
            [FromRoute] string licenceCode,
            CancellationToken cToken)
        {
            Dictionary<string, object> res = new();
            res.Add("Success", false);
            if (context.Licence != null && context.Logs != null)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    var licence = await context.Licence.FirstOrDefaultAsync(l => l.LicenceCode.Equals(licenceCode), cToken);
                    if (licence != null)
                    {
                        var req = jwt.ReadToken(token, licenceCode);
                        if (req.TryGetValue("Hash", out var obj) && obj is string hash)
                        {
                            await context.Logs.AddAsync(new Models.Log { LicenceId = licence.LicenceId, Hash = hash }, cToken);
                            if (string.IsNullOrEmpty(licence.Hash) || hash.Equals(licence.Hash))
                            {
                                licence.Hash = hash;
                                context.Licence.Update(licence);

                                res.Add("Hash", hash);
                                res["Success"] = true;
                            }
                            else
                            {
                                res.Add("Message", "Licence already bound to another machine");
                            }
                        }
                        await context.SaveChangesAsync(cToken);
                        transaction.Commit();
                    }
                }
                catch
                {
                    res.Add("Message", "Error has occurred, please contact administrator");
                    transaction.Rollback();
                }
            }
            return Results.Text(jwt.WriteToken(res)); //use salt in secret
        }
    }
}
