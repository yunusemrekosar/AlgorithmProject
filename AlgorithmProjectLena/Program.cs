using AlgorithmProjectLena;
using AlgorithmProjectLena.Model;

var durationBetweenLocationsMatrix = new List<DurationBetweenLocation>
{
    new DurationBetweenLocation { From = 'A', To = 'B', DurationMinutes = 15 },
    new DurationBetweenLocation { From = 'A', To = 'C', DurationMinutes = 20 },
    new DurationBetweenLocation { From = 'A', To = 'D', DurationMinutes = 11 },
    new DurationBetweenLocation { From = 'B', To = 'C', DurationMinutes = 5 },
    new DurationBetweenLocation { From = 'B', To = 'D', DurationMinutes = 25 },
    new DurationBetweenLocation { From = 'C', To = 'D', DurationMinutes = 25 }
};

List<Event> events = new List<Event>
{
    new Event { Id = 1, StartTime = "10:00", EndTime = "12:45", Location = 'A', Priority = 50 },
    new Event { Id = 2, StartTime = "10:00", EndTime = "11:00", Location = 'B', Priority = 30 },
    new Event { Id = 3, StartTime = "12:30", EndTime = "13:30", Location = 'A', Priority = 40 },
    new Event { Id = 4, StartTime = "14:30", EndTime = "16:00", Location = 'C', Priority = 70 },
    new Event { Id = 5, StartTime = "14:25", EndTime = "15:30", Location = 'B', Priority = 60 },
    new Event { Id = 6, StartTime = "13:00", EndTime = "14:00", Location = 'D', Priority = 80 }
};

var result = EventPlanner.ChooseEvent(durationBetweenLocationsMatrix, events);

//Burada aklımı "Katılınabilecek Maksimum Etkinlik Sayısı" kafamı karıştırdı çünkü aslında katılınabilecek maksimum etkinlik sayısını dönmüyorum kurallara uyarak toplanabilecek en fazla priority puanı topluyorum. Fakat örnekte de böyle yapıldığını gördüğüm için sormadım.
Console.WriteLine($"Katılınabilecek Maksimum Etkinlik Sayısı: {result.Count} \nKatılınabilecek Etkinliklerin ID'leri: {string.Join(',', result.Select(x => x.Id))} \nToplam Değer: {result.Sum(x => x.Priority)}");
