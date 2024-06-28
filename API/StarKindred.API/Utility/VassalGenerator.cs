using BenMakesGames.RandomHelpers;
using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;

namespace StarKindred.API.Utility;

public class VassalGenerator
{
    public static Vassal Generate(Random rng, int level, Species species, bool hasMilitarization)
    {
        return new Vassal()
        {
            Name = rng.Next(Names),
            Portrait = rng.Next(PortraitsForSpecies[species]),
            Level = level,
            Willpower = 1,
            Element = rng.NextEnumValue<Element>(),
            Sign = rng.NextEnumValue<AstrologicalSign>(),
            Nature = rng.NextEnumValue<Nature>(),
            Species = species,
            StatusEffects = !hasMilitarization ? null : new()
            {
                new() { Type = StatusEffectType.Power, Strength = 2 }
            }
        };
    }

    public static readonly Dictionary<Species, string[]> PortraitsForSpecies = new()
    {
        {
            Species.Human,
            new[]
            {
                "prince",
                "iru",
                "piin",
                "captain",
                "sword",
                "priestess",
                "nomad",
                "sailor",
                "eel",
                "adventurer",
                "eagle",
                "firstmate",
                "amelia",
                "herbalist",
                "hand",
                "bobby",
                "coat",
            }
        },
        {
            Species.Ruqu,
            new[]
            {
                "marquis",
                "swimmer",
                "merchant",
                "squire",
                "bride",
                "judge"
            }
        },
        {
            Species.Midine,
            new[]
            {
                "traveller",
                "knight-errant",
                "archer",
                "lion",
                "bear",
                "scout",
            }
        },
        {
            Species.Puturu,
            new[]
            {
                "envoy"
            }
        }
    };

    public static readonly string[] Names =
    {
        "Adaddu-Shalum", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Aho",
        "Akitu", // Babylonian New Year holiday
        "Albazi",
        "Amar-Sin", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Amata",
        "Amba-El", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Anshar", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Appan-Il",
        "Ardorach",
        "Arwia",
        "Ashlutum",
        "Asmaro",
        "Athra",
        "Balashi",
        "Barsawme",
        "Banunu",
        "Bel", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Beletsunu",
        "Belshazzar", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Belshimikka", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Bel-Shum-Usur", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Berosus",
        "Biridis", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Boram", // copilot-suggested
        "Caifas",
        "Celestia", // copilot-suggested
        "Cerasus-El", // copilot-suggested
        "Curus", // copilot-suggested
        "Dabra", // copilot-suggested
        "Dabra-Ea", // copilot-suggested w/ personal modification
        "Diimeritia",
        "Din-Turul", // I made this up; Din + -Turul suffix seen elsewhere
        "Dinu", // copilot-suggested
        "Doz", // copilot-suggested
        "Dwura",
        "Eannatum",
        "Ebru", // copilot-suggested
        "Ecna", // copilot-suggested
        "Eesho",
        "Edra", // copilot-suggested
        "Efra", // copilot-suggested
        "Ekka", // copilot-suggested
        "Ekran", // copilot-suggested
        "El", // copilot-suggested
        "Emmita",
        "Enheduana",
        "Enn", // copilot-suggested
        "Ettu",
        "Ezra", // copilot-suggested
        "Fara", // copilot-suggested
        "Fenra", // copilot-suggested
        "Fenra-Sin", // previous, plus a suffix I've seen before
        "Fenu", // copilot-suggested
        "Fenya", // copilot-suggested
        "Finna", // copilot-suggested
        "Firas", // copilot-suggested
        "Gabbara",
        "Gadatas",
        "Gemekaa",
        "Gewargis",
        "Goda", // copilot-suggested
        "Gomera", // copilot-suggested
        "Goram", // copilot-suggested
        "Gubaru",
        "Hammurabi",
        "Hann", // copilot-suggested
        "Hanuno",
        "Hara", // copilot-suggested
        "Hara-El", // copilot-suggested
        "Hebron", // copilot-suggested
        "Hemera", // copilot-suggested
        "Hesed", // copilot-suggested
        "Hisa", // copilot-suggested
        "Hod", // copilot-suggested
        "Hormuzd",
        "Hushmend", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Ia",
        "Iatum", // I made this one up
        "Ibbi-Adad",
        "Ibi", // extracted from a textsynth suggestion
        "Ibi-Atsi", // concatenated from two textsynth suggestions
        "Ibne", // copilot-suggested
        "Igal", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Igara", // copilot-suggested
        "Ili", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Ishep-Ana", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Ishme-Dagan", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Ishme-Ea",
        "Isimud", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Issavi",
        "Iwartas", // I made this one up
        "Izla",
        "Jabal", // copilot-suggested (and also biblical :P)
        "Jaram", // copilot-suggested
        "Jasen", // copilot-suggested
        "Jasen-El", // copilot-suggested
        "Jebe", // copilot-suggested
        "Jebre", // copilot-suggested
        "Job", // copilot-suggested (and also biblical :P)
        "Jod", // copilot-suggested
        "Jod-Aho", // copilot-suggested
        "Joshe", // copilot-suggested
        "Kabu", // copilot-suggested
        "Kalumtum",
        "Kan", // copilot-suggested
        "Khannah",
        "Khoshaba",
        "Ki", // copilot-suggested
        "Ko", // copilot-suggested
        "Ku-Aya",
        "Kugalis", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Laliya",
        "Lar-Aho", // copilot-suggested
        "Lilis",
        "Lilorach", // Lilis+ -orach suffix seen elsewhere
        "Lumiya", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Makara", // copilot-suggested
        "Malko",
        "Mazra", // copilot-suggested
        "Mekka", // copilot-suggested
        "Mylitta",
        "Nabu", // copilot-suggested
        "Nabua", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Nahrin",
        "Nahtum", // copilot-suggested
        "Nanshe-Kalum", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Naram-Sin",
        "Nazir",
        "Nebo", // copilot-suggested
        "Nebuchadnezzar",
        "Nektum", // copilot-suggested
        "Nesha", // copilot-suggested
        "Ninkurra", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Ninsun", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Nintu", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Nutesh",
        "Nur-Aya",
        "Odur", // copilot-suggested
        "Omarosa",
        "Oshana",
        "Pahtum", // copilot-suggested
        "Palkha",
        "Pardeeshur", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Puabi",
        "Puabu-Aya", // copilot-suggested
        "Rabbu",
        "Reshlutum", // I totally made this one up
        "Rimush",
        "Rishon", // copilot-suggested
        "Saba", // copilot-suggested
        "Samsi-Addu",
        "Samsu-Iluna", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Sarami-Zu", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Sarsurimutu", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Semiramis",
        "Shala-Kin", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Shalimoon",
        "Shamshi", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Shamsi-Adad", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Shu-Turul",
        "Sybella",
        "Tahira", // copilot-suggested
        "Takhana",
        "Tashlutum",
        "Teba", // copilot-suggested
        "Tebi", // copilot-suggested
        "Toram", // copilot-suggested
        "Tora", // copilot-suggested
        "Tu-Aya", // copilot-suggested
        "Ubbi-Adad", // copilot-suggested
        "Udun", // copilot-suggested
        "Ukubu",
        "Uru-Amurri", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Urukat", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Urukki", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Ushan", // copilot-suggested
        "Ushara", // copilot-suggested
        "Utu-Anu", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Uzuri", // from textsynth.com, with a prompt for Babylonian and Assyrian names
        "Waru", // I made this one up
        "Winhana", // I made this one up,
        "Yahatti-Il",
        "Yahtum", // copilot-suggested
        "Yonita",
        "Younan",
        "Zaia",
        "Zaiamoon", // I just combined Zaia + -moon from Shalimoon
        "Zaidu",
        "Zakiti",
        "Zakkum", // copilot-suggested
        "Zamir", // copilot-suggested
        "Zarai", // copilot-suggested
        "Zarath", // copilot-suggested
        "Zarath-Sin", // copilot-suggested
    };
}