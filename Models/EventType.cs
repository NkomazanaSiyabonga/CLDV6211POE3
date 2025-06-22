namespace CloudDevelopmentPOE1.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    public class EventType
    {
        public int EventTypeID { get; set; }
        public string Name { get; set; }

        public List<Event> Events { get; set; }


    }



}
