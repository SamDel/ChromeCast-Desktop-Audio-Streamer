namespace ChromeCast.Desktop.AudioStreamer.Classes
{
    public class ComboboxItem
    {
        public object Value { get; set; }

        public ComboboxItem(object value)
        {
            Value = value;
        }

        public override string ToString()
        {
            if (Value is SupportedStreamFormat)
                return Resource.Get(Value.ToString());

            return Value.ToString();
        }
    }
}
