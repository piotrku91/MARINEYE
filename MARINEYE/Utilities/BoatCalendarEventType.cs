namespace MARINEYE.Utilities
{
    public enum BoatCalendarEventType
    {
        Internal,
        Charter
    }

    static public class BoatCalendarEventTypeUtils
    {
        static private Dictionary<BoatCalendarEventType, string> _boatCalendarEventTypeDictionary = Enum.GetValues(typeof(BoatCalendarEventType))
                              .Cast<BoatCalendarEventType>()
                              .ToDictionary(state => state, state => state.ToString());

        static public string? GetBoatCalendarEventStateString(BoatCalendarEventType key) {
            if (!_boatCalendarEventTypeDictionary.ContainsKey(key)) {
                return null;
            }

            return _boatCalendarEventTypeDictionary[key];
        }

        static public List<string> GetBoatCalendarEventStateAllStrings() {
            if (_boatCalendarEventTypeDictionary.Count == 0) {
                return new List<string>();
            }

            return _boatCalendarEventTypeDictionary.Values.ToList();
        }
    }
}
