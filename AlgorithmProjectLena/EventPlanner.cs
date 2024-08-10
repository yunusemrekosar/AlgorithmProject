using AlgorithmProjectLena.Model;
using System.Data;

namespace AlgorithmProjectLena
{
    public static class EventPlanner
    {
        private static List<DurationBetweenLocation> _durationBetweenLocationsMinutesMatrix { get; set; } = new();
        private static Dictionary<int, (int value, List<int> choosedEvents)> dictResult = new();
        private static Dictionary<string, int> startTimeCount { get; set; } = new();
        private static List<Event> _events { get; set; } = new();

        private static List<List<Event>> matrixResult = new();

        public static List<Event> ChooseEvent(List<DurationBetweenLocation> durationBetweenLocationsMinutesMatrix, List<Event> events)
        {
            int haveToChoose = -1;
            var result = new List<Event>();

            _durationBetweenLocationsMinutesMatrix = durationBetweenLocationsMinutesMatrix;

            _events = events
             .OrderBy(x => TimeSpan.Parse(x.StartTime))
             .ThenByDescending(x => x.Priority)
             .ToList();

            startTimeCount = GetStartTimeCount();

            foreach (var item in _events)
            {
                //Burada "Müsait zamanda gidilebilecek birden fazla etkinlik var ise en yüksek önem derecesine sahip olan etkinlik seçilmelidir." cümlesine istinaden aynı anda birden fazla etkinlik var ise önceliği en yüksek etkinliğe gidilmesini sağlıyorum
                if (startTimeCount.GetValueOrDefault(item.StartTime) > 1)
                {
                    var a = _events.Where(x => x.StartTime == item.StartTime).OrderByDescending(x => x.Priority).First();
                    if (a.Id != item.Id)
                    {
                        startTimeCount[item.StartTime] -= 1;
                        matrixResult.Add(new List<Event>());

                        //Bu değişken de eğer aynı anda birden fazla etkinlik varsa birisine gidilmesi gerektiğinden daha yüksek öncelik puanı toplanabilecek etkinlik planlarını devre dışı bırakmak için tanımlandı ilgili işlemi 85. satırda yapıyorum
                        haveToChoose = _events.IndexOf(a); // düzeltildi
                        continue;
                    }
                }

                //Burada temel amacım hangi etkinliğe gidersem ardından hangi etkinliklere gidebileceğimi görebildiğim bir matris oluşturmak aşağıda iki etkinlik alanı arasındaki mekan arasındaki süreyi de hesaba katarak çıktığımda yetişebileceklerimi listeliyor ve matrise ekliyorum
                var items = _events.Where(x => TimeSpan.Parse(x.StartTime) >= TimeSpan.Parse(item.EndTime) + getDurationBetweenLocation(item, x.Location)).ToList();

                if (items.Any())
                    matrixResult.Add(items);
                else matrixResult.Add(new List<Event>());
            }

            for (var i = _events.Count - 1; i >= 0; i--)
            {
                if (matrixResult[i].Count == 0)
                    dictResult.Add(i, (_events[i].Priority, new List<int> { i }));

                else if (matrixResult[i].Count == 1)
                {
                    var a = matrixResult[i].First();
                    var ai = _events.IndexOf(a);
                    dictResult.Add(i, ((_events[i].Priority + a.Priority), new List<int> { i, ai }));
                }
                else
                {
                    List<int> evs = new List<int>();
                    List<(int value, List<int> evs)> values = new();

                    foreach (var item in matrixResult[i])
                    {
                        var indexof = _events.IndexOf(item);
                        var dict = dictResult.GetValueOrDefault(indexof);
                        // Burada ilginç bir olay yaşadığım için hashlemek durumunda kaldım :)
                        var ae = dict.choosedEvents.ToHashSet();
                        ae.Add(i);
                        values.Add((dict.value, ae.ToList()));
                    }

                    var maxValueItem = values.OrderByDescending(x => x.value).FirstOrDefault();
                    dictResult.Add(i, ((maxValueItem.value + _events[i].Priority), maxValueItem.evs));
                }
            }

            if (haveToChoose != -1)
            {
                var misseds = dictResult.Where(x => !x.Value.choosedEvents.Contains(haveToChoose)).Select(x => x.Key);
                foreach (var item in misseds)
                    dictResult[item] = (0, null);
            }

            var res = dictResult.OrderByDescending(x => x.Value.value).First();

            //Burada zaten algoritmanın time complexitysi O(n) olduğu için dışarıya daha derli toplu dönmesini istedim. Sadece indexleri dönerek bu foreachten kurtulabilirdim ama böylesi daha doğru geldi
            foreach (var i in res.Value.choosedEvents)
                result.Add(_events[i]);
            
            result.Reverse();

            return result;
        }

        //Bu method yine aynı anda var olan etkinlerden birisine gidilmesi kuralından dolayı var. Bu kuralı gerektiren durumların var olup olmadığına bunun döndüğü değerden varıyorum. İsim koymakta biraz zorlandım.
        private static Dictionary<string, int> GetStartTimeCount()
        {
            Dictionary<string, int> startTimeCount = new Dictionary<string, int>();
            startTimeCount.Add(_events[0].StartTime, 1);

            for (int j = 1; j < _events.Count; j++)
                if (startTimeCount.ContainsKey(_events[j].StartTime))
                    startTimeCount[_events[j].StartTime]++;
                else
                    startTimeCount.Add(_events[j].StartTime, 1);

            return startTimeCount;
        }

        private static TimeSpan getDurationBetweenLocation(Event e, char location)
        {
            if (e.Location == location)
                return TimeSpan.Zero;

            int minute = _durationBetweenLocationsMinutesMatrix.Find(x =>
            (x.From == location && x.To == e.Location) ||
            (x.From == e.Location && x.To == location)).DurationMinutes;

            return TimeSpan.FromMinutes(minute);
        }
    }
}
