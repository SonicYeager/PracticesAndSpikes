using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppTesty.Dto
{
    internal record ChuckNorrisJokeDto(
        string Id,
        IEnumerable<string> Categories,
        DateTime Created_At,
        DateTime Updated_At,
        Uri Icon_Url,
        Uri Url,
        string Value);
}
