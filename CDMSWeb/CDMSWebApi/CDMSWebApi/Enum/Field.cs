namespace AVEVA.CDMS.WebApi
{
    using System;

    public class Field
    {
        private string fieldname;
        private string fieldvalue;

        public Field()
        {
        }

        public Field(string fieldname, string fieldvalue)
        {
            this.fieldname = fieldname;
            this.fieldvalue = fieldvalue;
        }

        public bool Equals(Field field)
        {
            if (field == null)
            {
                return false;
            }
            return ((field.fieldname == this.fieldname) && (field.fieldvalue == this.fieldvalue));
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!(obj is Field))
            {
                return false;
            }
            Field field = (Field) obj;
            return ((field.fieldvalue == this.fieldvalue) && (field.fieldname == this.fieldname));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public string FieldName
        {
            get
            {
                return this.fieldname;
            }
            set
            {
                this.fieldname = value;
            }
        }

        public string FieldValue
        {
            get
            {
                return this.fieldvalue;
            }
            set
            {
                this.fieldvalue = value;
            }
        }
    }
}

