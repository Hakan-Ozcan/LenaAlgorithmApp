using LenaAlgorithmApp;

public class EventScheduler
{
    private List<Event> events;
    private List<DurationMatrixEntry> durationMatrix;

    public EventScheduler(List<Event> events, List<DurationMatrixEntry> durationMatrix)
    {
        this.events = events;
        this.durationMatrix = durationMatrix;
    }

    public (List<int> selectedEventIds, int totalPriority) FindOptimalEvents()
    {
        //Öncelikle Etkinlikleri başlangıç zamanına göre sıralarız
        events = events.OrderBy(e => e.StartTime).ToList();

        List<int> selectedEventIds = new List<int>();
        HashSet<string> visitedLocations = new HashSet<string>();
        TimeSpan currentTime = TimeSpan.Zero;

        foreach (var ev in events)
        {
            // Eğer mekan daha önce ziyaret edilmediyse ve şuanki zaman etkinlik başlama saatinden küçük yada eşitse
            if (!visitedLocations.Contains(ev.Location) && currentTime <= ev.StartTime)
            {
           
                //canAttend o anki durumda etkinliğe katılıp katılamayacağımızı belirlemek için kullanılır
                bool canAttend = true;
                // Mekanlara gidebiliyor muyuz?
                foreach (var entry in durationMatrix.Where(d => d.From == ev.Location))
                {
                    //o anki saate ,yukarda elde ettiğimiz entry'nin DurationMinutes'ını eklersek ve bu etkinlik başlama saatinden büyük olursa etkinliğe gidemeyiz canAttend'e false atanır
                    if (currentTime + TimeSpan.FromMinutes(entry.DurationMinutes) > ev.StartTime)
                    {
                        canAttend = false;
                        break;
                    }
                    currentTime += TimeSpan.FromMinutes(entry.DurationMinutes);
                }

                if (canAttend)
                {
                    selectedEventIds.Add(ev.Id);
                    visitedLocations.Add(ev.Location);
                    //bi etkinliğe gittiysek o etkinliğin bitiş saatini current time'a atıyoruz.
                    currentTime = ev.EndTime;
                }
            }
        }
        //seçilen etkinliklerin toplam öncelik değerini hesaplar
        int totalPriority = events.Where(e => selectedEventIds.Contains(e.Id)).Sum(e => e.Priority);

        return (selectedEventIds, totalPriority);
    }
}