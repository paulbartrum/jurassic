using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Performance
{

    /// <summary>
    /// Benchmarks the global JSON object.
    /// </summary>
    [TestClass]
    public class JSONBenchmarks
    {
        private string jsonString1 = @"
    {
        ""serviceDescription"" : """", 
        ""mapName"" : ""Time Zone activity"", 
        ""description"" : """", 
        ""copyrightText"" : """", 
        ""layers"" : [
        {
            ""id"" : 0, 
            ""name"" : ""Time_ZoneAnno"", 
            ""parentLayerId"" : -1, 
            ""defaultVisibility"" : true, 
            ""subLayerIds"" : [1]
        }, 
        {
            ""id"" : 1, 
            ""name"" : ""Default"", 
            ""parentLayerId"" : 0, 
            ""defaultVisibility"" : true, 
            ""subLayerIds"" : null
        }, 
        {
            ""id"" : 2, 
            ""name"" : ""Cities for timezone activities"", 
            ""parentLayerId"" : -1, 
            ""defaultVisibility"" : true, 
            ""subLayerIds"" : null
        }, 
        {
            ""id"" : 3, 
            ""name"" : ""Countries"", 
            ""parentLayerId"" : -1, 
            ""defaultVisibility"" : true, 
            ""subLayerIds"" : null
        }, 
        {
            ""id"" : 4, 
            ""name"" : ""Time Zone"", 
            ""parentLayerId"" : -1, 
            ""defaultVisibility"" : true, 
            ""subLayerIds"" : null
        }
        ], 
        ""spatialReference"" : {
        ""wkid"" : 4326
        }, 
        ""singleFusedMapCache"" : false, 
        ""initialExtent"" : {
        ""xmin"" : -207.006995449869, 
        ""ymin"" : -176.917674606978, 
        ""xmax"" : 207.410519682088, 
        ""ymax"" : 178.111556452722, 
        ""spatialReference"" : {
            ""wkid"" : 4326
        }
        }, 
        ""fullExtent"" : {
        ""xmin"" : -207.007003180235, 
        ""ymin"" : -99.0596940922872, 
        ""xmax"" : 207.410527412454, 
        ""ymax"" : 100.253575938032, 
        ""spatialReference"" : {
            ""wkid"" : 4326
        }
        }, 
        ""units"" : ""esriDecimalDegrees"", 
        ""documentInfo"" : {
        ""Title"" : ""Oylmpics_Time_Zone"", 
        ""Author"" : ""cet"", 
        ""Comments"" : """", 
        ""Subject"" : """", 
        ""Category"" : """", 
        ""Keywords"" : """"
        }
    }";

        [TestMethod]
        public void stringify1()
        {
            // 27100 inner loops/sec
            var engine = new Jurassic.ScriptEngine();
            var jsonObject = Jurassic.Library.JSONObject.Parse(engine, jsonString1);
            TestUtils.Benchmark(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    string str = Jurassic.Library.JSONObject.Stringify(engine, jsonObject);
                }
            });
        }

        private string jsonString2 = @"[{""id"": ""0001"",""type"": ""donut"",""name"": ""Cake"",""ppu"": 0.55,""batters"":{""batter"":[{ ""id"": ""1001"", ""type"": ""Regular"" },{ ""id"": ""1002"", ""type"": ""Chocolate"" },{ ""id"": ""1003"", ""type"": ""Blueberry"" },{ ""id"": ""1004"", ""type"": ""Devil's Food"" }]},""topping"":[{ ""id"": ""5001"", ""type"": ""None"" },{ ""id"": ""5002"", ""type"": ""Glazed"" },{ ""id"": ""5005"", ""type"": ""Sugar"" },{ ""id"": ""5007"", ""type"": ""Powdered Sugar"" },{ ""id"": ""5006"", ""type"": ""Chocolate with Sprinkles"" },{ ""id"": ""5003"", ""type"": ""Chocolate"" },{ ""id"": ""5004"", ""type"": ""Maple"" }]},{""id"": ""0002"",""type"": ""donut"",""name"": ""Raised"",""ppu"": 0.55,""batters"":{""batter"":[{ ""id"": ""1001"", ""type"": ""Regular"" }]},""topping"":[{ ""id"": ""5001"", ""type"": ""None"" },{ ""id"": ""5002"", ""type"": ""Glazed"" },{ ""id"": ""5005"", ""type"": ""Sugar"" },{ ""id"": ""5003"", ""type"": ""Chocolate"" },{ ""id"": ""5004"", ""type"": ""Maple"" }]},{""id"": ""0003"",""type"": ""donut"",""name"": ""Old Fashioned"",""ppu"": 0.55,""batters"":{""batter"":[{ ""id"": ""1001"", ""type"": ""Regular"" },{ ""id"": ""1002"", ""type"": ""Chocolate"" }]},""topping"":[{ ""id"": ""5001"", ""type"": ""None"" },{ ""id"": ""5002"", ""type"": ""Glazed"" },{ ""id"": ""5003"", ""type"": ""Chocolate"" },{ ""id"": ""5004"", ""type"": ""Maple"" }]}]";

        [TestMethod]
        public void parse2()
        {
            // 19600 inner loops/sec
            var engine = new Jurassic.ScriptEngine();
            TestUtils.Benchmark(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    object obj = Jurassic.Library.JSONObject.Parse(engine, jsonString2);
                }
            });
        }

        [TestMethod]
        public void stringify2()
        {
            // 22100 inner loops/sec
            var engine = new Jurassic.ScriptEngine();
            var jsonObject = Jurassic.Library.JSONObject.Parse(engine, jsonString2);
            TestUtils.Benchmark(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    string str = Jurassic.Library.JSONObject.Stringify(engine, jsonObject);
                }
            });
        }

        private string jsonString3 = @"
    [
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Empire Burlesque"",
		    ""artist"": ""Bob Dylan"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 10.90,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Hide your heart"",
		    ""artist"": ""Bonnie Tyler"",
		    ""country"": ""UK"",
		    ""company"": ""CBS Records"",
		    ""price"": 9.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Greatest Hits"",
		    ""artist"": ""Dolly Parton"",
		    ""country"": ""USA"",
		    ""company"": ""RCA"",
		    ""price"": 9.90,
		    ""year"": 1982
	    },
	    {
		    ""title"": ""Still got the blues"",
		    ""artist"": ""Gary Moore"",
		    ""country"": ""UK"",
		    ""company"": ""Virgin records"",
		    ""price"": 10.20,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Eros"",
		    ""artist"": ""Eros Ramazzotti"",
		    ""country"": ""EU"",
		    ""company"": ""BMG"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""One night only"",
		    ""artist"": ""Bee Gees"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 10.90,
		    ""year"": 1998
	    },
	    {
		    ""title"": ""Sylvias Mother"",
		    ""artist"": ""Dr.Hook"",
		    ""country"": ""UK"",
		    ""company"": ""CBS"",
		    ""price"": 8.10,
		    ""year"": 1973
	    },
	    {
		    ""title"": ""Maggie May"",
		    ""artist"": ""Rod Stewart"",
		    ""country"": ""UK"",
		    ""company"": ""Pickwick"",
		    ""price"": 8.50,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Romanza"",
		    ""artist"": ""Andrea Bocelli"",
		    ""country"": ""EU"",
		    ""company"": ""Polydor"",
		    ""price"": 10.80,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""When a man loves a woman"",
		    ""artist"": ""Percy Sledge"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 8.70,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Black angel"",
		    ""artist"": ""Savage Rose"",
		    ""country"": ""EU"",
		    ""company"": ""Mega"",
		    ""price"": 10.90,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""1999 Grammy Nominees"",
		    ""artist"": ""Many"",
		    ""country"": ""USA"",
		    ""company"": ""Grammy"",
		    ""price"": 10.20,
		    ""year"": 1999
	    },
	    {
		    ""title"": ""For the good times"",
		    ""artist"": ""Kenny Rogers"",
		    ""country"": ""UK"",
		    ""company"": ""Mucik Master"",
		    ""price"": 8.70,
		    ""year"": 1995
	    },
	    {
		    ""title"": ""Big Willie style"",
		    ""artist"": ""Will Smith"",
		    ""country"": ""USA"",
		    ""company"": ""Columbia"",
		    ""price"": 9.90,
		    ""year"": 1997
	    },
	    {
		    ""title"": ""Tupelo Honey"",
		    ""artist"": ""Van Morrison"",
		    ""country"": ""UK"",
		    ""company"": ""Polydor"",
		    ""price"": 8.20,
		    ""year"": 1971
	    },
	    {
		    ""title"": ""Soulsville"",
		    ""artist"": ""Jorn Hoel"",
		    ""country"": ""Norway"",
		    ""company"": ""WEA"",
		    ""price"": 7.90,
		    ""year"": 1996
	    },
	    {
		    ""title"": ""The very best of"",
		    ""artist"": ""Cat Stevens"",
		    ""country"": ""UK"",
		    ""company"": ""Island"",
		    ""price"": 8.90,
		    ""year"": 1990
	    },
	    {
		    ""title"": ""Stop"",
		    ""artist"": ""Sam Brown"",
		    ""country"": ""UK"",
		    ""company"": ""A and M"",
		    ""price"": 8.90,
		    ""year"": 1988
	    },
	    {
		    ""title"": ""Bridge of Spies"",
		    ""artist"": ""T`Pau"",
		    ""country"": ""UK"",
		    ""company"": ""Siren"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Private Dancer"",
		    ""artist"": ""Tina Turner"",
		    ""country"": ""UK"",
		    ""company"": ""Capitol"",
		    ""price"": 8.90,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Midt om natten"",
		    ""artist"": ""Kim Larsen"",
		    ""country"": ""EU"",
		    ""company"": ""Medley"",
		    ""price"": 7.80,
		    ""year"": 1983
	    },
	    {
		    ""title"": ""Pavarotti Gala Concert"",
		    ""artist"": ""Luciano Pavarotti"",
		    ""country"": ""UK"",
		    ""company"": ""DECCA"",
		    ""price"": 9.90,
		    ""year"": 1991
	    },
	    {
		    ""title"": ""The dock of the bay"",
		    ""artist"": ""Otis Redding"",
		    ""country"": ""USA"",
		    ""company"": ""Atlantic"",
		    ""price"": 7.90,
		    ""year"": 1987
	    },
	    {
		    ""title"": ""Picture book"",
		    ""artist"": ""Simply Red"",
		    ""country"": ""EU"",
		    ""company"": ""Elektra"",
		    ""price"": 7.20,
		    ""year"": 1985
	    },
	    {
		    ""title"": ""Red"",
		    ""artist"": ""The Communards"",
		    ""country"": ""UK"",
		    ""company"": ""London"",
		    ""price"": 7.80,
		    ""year"": 1987
	    }
    ]";

        [TestMethod]
        public void parse3()
        {
            // 110 inner loops/sec
            var engine = new Jurassic.ScriptEngine();
            TestUtils.Benchmark(() =>
            {
                object obj = Jurassic.Library.JSONObject.Parse(engine, jsonString3);
            });
        }

        [TestMethod]
        public void stringify3()
        {
            // 166 inner loops/sec
            var engine = new Jurassic.ScriptEngine();
            var jsonObject = Jurassic.Library.JSONObject.Parse(engine, jsonString3);
            TestUtils.Benchmark(() =>
            {
                string str = Jurassic.Library.JSONObject.Stringify(engine, jsonObject);
            });
        }
    }

}