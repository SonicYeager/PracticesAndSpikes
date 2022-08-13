using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppTesty.Dto
{
    internal record ChuckNorrisJokeDto(
        string id,
        IEnumerable<string> categories,
        string created_at,
        string updated_at,
        Uri icon_url,
        Uri url,
        string value);
}
