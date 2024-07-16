namespace webHookForCrm.Model
{
    public class WebhookRequest
    {
        public string Object { get; set; }
        public List<Entry> Entry { get; set; }
    }

    public class Entry
    {
        public string Id { get; set; }
        public List<Change> Changes { get; set; }
    }

    public class Change
    {
        public Value Value { get; set; }
        public string Field { get; set; }
    }

    public class Value
    {
        public string Messaging_Product { get; set; }
        public Metadata Metadata { get; set; }
        public List<Contact> Contacts { get; set; }
        public List<Message> Messages { get; set; }
    }

    public class Metadata
    {
        public string Display_phone_number { get; set; }
        public string Phone_number_id { get; set; }
    }

    public class Contact
    {
        public Profile Profile { get; set; }
        public string Wa_Id { get; set; }
    }

    public class Profile
    {
        public string Name { get; set; }
    }

    public class Message
    {
        public string From { get; set; }
        public string Id { get; set; }
        public string Timestamp { get; set; }
        public Text Text { get; set; }
        public string Type { get; set; }
    }

    public class Text
    {
        public string Body { get; set; }
    }
}
