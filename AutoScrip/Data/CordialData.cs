using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScrip.Data;

public class Cordial
{
    public uint CordialGP;
    public uint CordialId;

    public Cordial(uint cordialGP, uint cordialId)
    {
        CordialGP = cordialGP;
        CordialId = cordialId;
    }
}

public class CordialPriorityTable
{
    public static readonly List<Cordial> Table = new()
    {
        new Cordial(400, 12669),
        new Cordial(350, 1006141),
        new Cordial(300, 6141),
        new Cordial(200, 1016911),
        new Cordial(150, 16911)
    };
}

public class CordialInvertedPriorityTable
{
    public static readonly List<Cordial> Table = new()
    {
        new Cordial(150, 16911),
        new Cordial(200, 1016911),
        new Cordial(300, 6141),
        new Cordial(350, 1006141),
        new Cordial(400, 12669)
    };
}
