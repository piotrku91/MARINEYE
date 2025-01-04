namespace MARINEYE.Utilities
{
    public enum BoatCalendarEventState
    {
        Reserved,
        Confirmed
    }

    static public class BoatCalendarEventStateUtils
    {
        static private Dictionary<BoatCalendarEventState, string> _boatCalendarEventStateDictionary = new Dictionary<BoatCalendarEventState, string>
            {
                { BoatCalendarEventState.Reserved, "Rezerwacja" },
                { BoatCalendarEventState.Confirmed, "Potwierdzony" }
            };

        static public string? GetBoatCalendarEventStateString(BoatCalendarEventState key) {
            if (!_boatCalendarEventStateDictionary.ContainsKey(key)) {
                return null;
            }

            return _boatCalendarEventStateDictionary[key];
        }

        static public List<string> GetBoatCalendarEventStateAllStrings() {
            if (_boatCalendarEventStateDictionary.Count == 0) {
                return new List<string>();
            }

            return _boatCalendarEventStateDictionary.Values.ToList();
        }
    }
}
