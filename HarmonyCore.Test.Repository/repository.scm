 
;  SYNERGY DATA LANGUAGE OUTPUT
;
;  REPOSITORY     : C:\Users\hippi\source\repos\HarmonyCore\HarmonyCore.Test.Repository\bin\Debug\rpsmain.ism
;                 : C:\Users\hippi\source\repos\HarmonyCore\HarmonyCore.Test.Repository\bin\Debug\rpstext.ism
;                 : Version 11.1.1e
;
;  GENERATED      : 28-OCT-2020, 13:14:11
;                 : Version 11.1.1f
;  EXPORT OPTIONS : [ALL] 
 
 
Format PHONE   Type NUMERIC   "(XXX) XXX-XXXX"   Justify RIGHT
 
Enumeration COLOR
   Description "enum for color"
   Members RED 0, BLUE 1, GREEN 2, YELLOW 3
 
Enumeration DAYOFWEEK
   Description "enum for days of week"
   Members SUNDAY 1, MONDAY 2, TUESDAY 3, WEDNESDAY 4, THURSDAY 5,
          FRIDAY 6, SATURDAY 7
 
Enumeration METHOD_STATUS
   Description "Method return status code"
   Members SUCCESS 0, WARNING 1, ERROR 2, FATAL 3
 
Enumeration MYCOLOR
   Description "Another color"
   Members RED, BLUE, GREEN, YELLOW
 
Template PARENT_NAME_TEMPLATE   Type ALPHA   Size 30
 
Template DATE   Type DATE   Size 8   Stored YYYYMMDD
   Description "YYYYMMDD date"
   Prompt "Date"   Report Heading "Date"   ODBC Name DATE
 
Template NAME_TEMPLATE   Parent PARENT_NAME_TEMPLATE
 
Template PHONE   Type DECIMAL   Size 10
   Description "Phone number"
   Prompt "Phone #:"   User Text "HARMONY_AS_STRING"   Format PHONE
   Report Heading "Phone Number"
 
Structure STRU_A   DBL ISAM
   Description "Structure A for testing"
 
Field ALPHA_20   Type ALPHA   Size 20
   Description "Alpha 20"
 
Field DEC_5   Type DECIMAL   Size 5
 
Field DEC_15   Type DECIMAL   Size 15
 
Field IDEC_31   Type DECIMAL   Size 3   Precision 1
 
Field IDEC_144   Type DECIMAL   Size 14   Precision 4
 
Field I_1   Type INTEGER   Size 1
 
Field I_2   Type INTEGER   Size 2
 
Field I_4   Type INTEGER   Size 4
 
Field I_8   Type INTEGER   Size 8
 
Key ALPHA   ACCESS   Order ASCENDING   Dups NO
   Segment FIELD   ALPHA_20
 
Structure STROPTSINNER   DBL ISAM
   Description "Struct Opts inner structure"
 
Field KEY_INNNER   Type DECIMAL   Size 5
 
Field DESC_INNER   Type ALPHA   Size 25
 
Field AMOUNT_INNER   Type DECIMAL   Size 9   Precision 2
 
Structure MSC1   ASCII
   Description "test"
 
Field MSC1_AL10   Type ALPHA   Size 10
 
Field MSC1_DE5   Type DECIMAL   Size 5
 
Structure MSC2   DBL ISAM
   Description "test"
 
Field MSC2_DE5   Type DECIMAL   Size 5
 
Field MSC2_STRUCTOFMSC1   Type STRUCT   Size 15   Struct MSC1
 
Field MSC2_AL20   Type ALPHA   Size 20
 
Structure GPC3   DBL ISAM
   Description "Third Structure"
 
Field FLD_1F   Type INTEGER   Size 4
   Description "GPC3.FLD_1F"
 
Field FLD_2F   Type INTEGER   Size 4
   Description "GPC3.FLD_2F"
 
Field FLD_3F   Type INTEGER   Size 4
   Description "GPC3.FLD_3F"
 
Field FLD_4F   Type INTEGER   Size 4
   Description "GPC3.FLD_4F"
 
Alias AL_GPC3   Structure GPC3
   Alias AL_FLD_1F   Field FLD_1F
   Alias AL_FLD_2F   Field FLD_2F
   Alias AL_FLD_3F   Field FLD_3F
   Alias AL_FLD_4F   Field FLD_4F
   Alias AL_FLD_4F2   Field FLD_4F
 
Structure GPC6   DBL ISAM
   Description "Sixth Structure"
 
Field FIELD_ONE   Type ALPHA   Size 3
 
Field FIELD_TWO   Template PARENT_NAME_TEMPLATE
 
Field FIELD_THREE   Template NAME_TEMPLATE
 
Field FIELD_FOUR   Type DECIMAL   Size 14
 
Group GROUP_ONE   Type ALPHA
 
   Field MEMBER_ONE   Template PARENT_NAME_TEMPLATE
 
   Field MEMBER_TWO   Template NAME_TEMPLATE
 
   Field MEMBER_THREE   Type STRUCT   Size 16   Struct GPC3
 
Endgroup
 
Alias AL_GPC6   Structure GPC6
 
Structure GPC4   DBL ISAM
   Description "Fourth Structure"
 
Field FLD_1G   Type ALPHA   Size 3
   Description "GPC4.FLD_1G"
 
Field FLD_2G   Type INTEGER   Size 2   Dimension 4
   Description "GPC4.FLD_2G"
 
Field STRUCT_1G   Type STRUCT   Size 16   Struct GPC3
   Description "GPC4.STRUCT_1G"
 
Field FLD_3G   Type ALPHA   Size 3   Dimension 3
   Description "GPC4.FLD_3G"
 
Field FLD_4G   Type DECIMAL   Size 13
   Description "GPC4.FLD_4G"
 
Alias AL_GPC4   Structure GPC4
 
Structure GPC2   DBL ISAM
   Description "Second Structure"
 
Field FLD_1C   Type ALPHA   Size 5
   Description "GPC2.FLD_1C"
 
Field FLD_2C   Type STRUCT   Size 16   Struct GPC3
   Description "GPC2_FLD_2C"
 
Group GRP_1C   Type ALPHA
   Description "GPC2.GRP_1C"
 
   Field FLD_1D   Type DECIMAL   Size 10
      Description "GPC2.GRP_1C.FLD_1D"
 
   Field FLD_2D   Type DECIMAL   Size 10
      Description "GPC2.GRP_1C.FLD_2D"
 
Endgroup
 
Alias AL_GPC2   Structure GPC2
 
Structure ADDRESS   DBL ISAM
   Description "Address layout"
 
Group STREET   Type ALPHA   Dimension 3
 
   Field LINE   Type ALPHA   Size 40
 
Endgroup
 
Field CITY   Type ALPHA   Size 30
 
Field STATE   Type ALPHA   Size 2
 
Field COUNTRY   Type ALPHA   Size 10
 
Field POSTAL_CODE   Type ALPHA   Size 20
 
Structure DBPUBLISHER   DBL ISAM
   Description "Publisher file"
 
Field PUBLISHERID   Type ALPHA   Size 10
   Description "Unique publisher identifier"
 
Field NAME   Type ALPHA   Size 50
   Description "Publisher's name"
 
Group PUB_ADDR   Reference ADDRESS   Type ALPHA
 
Key PUBLISHERID   ACCESS   Order ASCENDING   Dups NO
   Segment FIELD   PUBLISHERID
 
Structure DBORDER   DBL ISAM
   Description "Order file"
 
Field ORDERNO   Type DECIMAL   Size 10
   Description "Unique order number"
 
Field CUST_NAME   Type ALPHA   Size 40
   Description "Customer's name"
 
Group ADRESSES   Reference ADDRESS   Type ALPHA   Dimension 2
   Description "1 - billing; 2 - shipping"
 
Key ORDERNO   ACCESS   Order ASCENDING   Dups NO
   Segment FIELD   ORDERNO
 
Structure BINARYTEST   DBL ISAM
   Description "Test of an array of binary byte arrays"
 
Field KEYNO   Type DECIMAL   Size 6
 
Field DESC   Type ALPHA   Size 25
 
Field BINARRAY   Type ALPHA   Size 10   Stored BINARY   Dimension 5
 
Field NUMBER   Type DECIMAL   Size 9
 
Structure BOOLEANSTR   DBL ISAM
   Description "Test boolean fields in a structure"
 
Field AL10   Type ALPHA   Size 10
 
Field TYPEBOOL1   Type BOOLEAN   Size 4
 
Field INT1CTBOOL   Type INTEGER   Size 1   Coerced Type BOOLEAN
 
Field INT2CTBOOL   Type INTEGER   Size 2   Coerced Type BOOLEAN
 
Field INT4CTBOOL   Type INTEGER   Size 4   Coerced Type BOOLEAN
 
Field INT8CTBOOL   Type INTEGER   Size 8   Coerced Type BOOLEAN
 
Field DEC3CTBOOL   Type DECIMAL   Size 3   Coerced Type BOOLEAN
 
Field DEC6CTBOOL   Type DECIMAL   Size 6   Coerced Type BOOLEAN
 
Field DEC9CTBOOL   Type DECIMAL   Size 9   Coerced Type BOOLEAN
 
Field DEC13CTBOOL   Type DECIMAL   Size 13   Coerced Type BOOLEAN
 
Field DEC17CTBOOL   Type DECIMAL   Size 17   Coerced Type BOOLEAN
 
Structure COERCESTRUCTURE   DBL ISAM
   Description "Coerced type test structure"
 
Field DECINT   Type DECIMAL   Size 6
 
Field DECBOOL   Type DECIMAL   Size 1   Coerced Type BOOLEAN
 
Field DECBYTE   Type DECIMAL   Size 1   Coerced Type BYTE
 
Field DECSHORT   Type DECIMAL   Size 2   Coerced Type SHORT
 
Field DECLONG   Type DECIMAL   Size 10   Coerced Type LONG
 
Field DECSBYTE   Type DECIMAL   Size 1   Coerced Type SBYTE
 
Field DECUINT   Type DECIMAL   Size 5   Coerced Type UINT
 
Field DECUSHORT   Type DECIMAL   Size 2   Coerced Type SHORT
 
Field DECULONG   Type DECIMAL   Size 10   Coerced Type LONG
 
Field DECDATETIME   Type DATE   Size 8   Stored YYYYMMDD
 
Field NULLDATETIME   Type DATE   Size 8   Stored YYYYMMDD
   Coerced Type NULLABLE_DATETIME
 
Field IDDECIMAL   Type DECIMAL   Size 7   Precision 2
 
Field IDDOUBLE   Type DECIMAL   Size 7   Precision 2
   Coerced Type DOUBLE
 
Field IDFLOAT   Type DECIMAL   Size 7   Precision 2
   Coerced Type FLOAT
 
Field INTINT   Type INTEGER   Size 4
 
Field INTBOOL   Type INTEGER   Size 1   Coerced Type BOOLEAN
 
Field INTBYTE   Type INTEGER   Size 1   Coerced Type BYTE
 
Field INTSHORT   Type INTEGER   Size 2   Coerced Type SHORT
 
Field INTLONG   Type INTEGER   Size 8   Coerced Type LONG
 
Field INTSBYTE   Type INTEGER   Size 1   Coerced Type SBYTE
 
Field INTUINT   Type INTEGER   Size 4   Coerced Type UINT
 
Field INTUSHORT   Type INTEGER   Size 2   Coerced Type SHORT
 
Field INTULONG   Type INTEGER   Size 8   Coerced Type LONG
 
Structure COERCE_TEST   DBL ISAM
   Description "Coercion test structure"
 
Field A30   Type ALPHA   Size 30
   Description "Alpha 30"
 
Field I1   Type INTEGER   Size 1
   Description "Integer 1"
 
Field I2   Type INTEGER   Size 2
   Description "Integer 2"
 
Field I4   Type INTEGER   Size 4
   Description "Integer 4"
 
Field I8   Type INTEGER   Size 8
   Description "Integer 8"
 
Field D4   Type DECIMAL   Size 4
   Description "Decimal 4"
 
Field D9   Type DECIMAL   Size 9
   Description "Decimal 9"
 
Field D10   Type DECIMAL   Size 10
   Description "Decimal 10"
 
Group GRP   Type ALPHA   Size 30
   Description "Group"
 
   Field GRP_A30   Type ALPHA   Size 30
 
Endgroup
 
Field AR3A30   Type ALPHA   Size 30   Dimension 3
   Description "Array of 3 alpha 30"
 
Field AR3I1   Type INTEGER   Size 1   Dimension 3
   Description "Array of 3 integer 1"
 
Field AR3I2   Type INTEGER   Size 2   Dimension 3
   Description "Array of 3 integer 2"
 
Field AR3I4   Type INTEGER   Size 4   Dimension 3
   Description "Array of 3 integer 4"
 
Field AR3I8   Type INTEGER   Size 8   Dimension 3
   Description "Array of 3 integer 8"
 
Field AR3D4   Type DECIMAL   Size 4   Dimension 3
   Description "Array of 3 decimal 4"
 
Field AR3D9   Type DECIMAL   Size 9   Dimension 3
   Description "Array of 3 decimal 9"
 
Field AR3D10   Type DECIMAL   Size 10   Dimension 3
   Description "Array of 3 decimal 10"
 
Group AR3GRP   Type ALPHA   Size 30   Dimension 3
   Description "Array of 3 group"
 
   Field GRP2_A30   Type ALPHA   Size 30
 
Endgroup
 
Structure CUSTOMER   DBL ISAM
   Description "Tmp test structure"
 
Field FAKEFIELD   Type ALPHA   Size 100
 
Field TMPAL5   Type ALPHA   Size 5
   Readonly
 
Field TMPAL5A   Type ALPHA   Size 5
 
Field TMPD10   Type DECIMAL   Size 10
   Readonly
 
Structure CUSTOMERS   DBL ISAM
   Description "Customer record"
 
Field CUSTOMER_NUMBER   Type DECIMAL   Size 6
   Description "Customer number"
   Long Description
      "SAMPLE_DATA=355232;"
   Report Just LEFT   Input Just LEFT
   Required
 
Field NAME   Type ALPHA   Size 30
   Description "Customer name"
   Long Description
      "SAMPLE_DATA=Abe's Nursery;"
   Required
 
Field STREET   Type ALPHA   Size 25
   Description "Street address"
   Long Description
      "SAMPLE_DATA=1032 Main Street;"
 
Field CITY   Type ALPHA   Size 20
   Description "City"
   Long Description
      "SAMPLE_DATA=Springfield;"
 
Field STATE   Type ALPHA   Size 2
   Description "State"
   Long Description
      "SAMPLE_DATA=MO;"
   Uppercase
 
Field ZIP_CODE   Type DECIMAL   Size 9
   Description "Zip code"
   Long Description
      "SAMPLE_DATA=64127;"
   Report Just LEFT   Input Just LEFT
 
Field CONTACT   Type ALPHA   Size 25
   Description "Contact name"
   Long Description
      "SAMPLE_DATA=Abe Albright;"
 
Field PHONE   Template PHONE
   Description "Phone number"
   Long Description
      "SAMPLE_DATA=(555) 123-4567;"
 
Field FAX   Template PHONE
   Description "Fax number"
   Long Description
      "SAMPLE_DATA=(555) 987-6543;"
 
Field FAVORITE_ITEM   Type DECIMAL   Size 6
   Description "Customers favorite item"
   Long Description
      "SAMPLE_DATA=7;"
   Report Just LEFT   Input Just LEFT
 
Field PAYMENT_TERMS_CODE   Type ALPHA   Size 2
   Description "Payment terms code"
   Long Description
      "SAMPLE_DATA=30;"
   Selection List 0 0 0  Entries "CA", "30", "60", "90"
 
Field TAX_ID   Type DECIMAL   Size 9
   Description "Customers tax ID number"
   Long Description
      "SAMPLE_DATA=546874521;"
      "HARMONY_ROLES=Manager;"
   Required
 
Field CREDIT_LIMIT   Type DECIMAL   Size 7   Precision 2
   Description "Credit limit"
   Long Description
      "SAMPLE_DATA=5000;"
 
Key CUSTOMER_NUMBER   ACCESS   Order ASCENDING   Dups NO
   Segment FIELD   CUSTOMER_NUMBER  SegType DECIMAL
 
Key STATE   ACCESS   Order ASCENDING   Dups YES   Insert END   Modifiable YES
   Krf 001
   Description "State"
   Segment FIELD   STATE  SegType ALPHA  SegOrder ASCENDING
 
Key ZIP   ACCESS   Order ASCENDING   Dups YES   Insert END   Modifiable YES
   Krf 002
   Description "Zip code"
   Segment FIELD   ZIP_CODE  SegType DECIMAL  SegOrder ASCENDING
 
Key PAYMENT_TERMS   ACCESS   Order ASCENDING   Dups YES   Insert END
   Modifiable YES   Krf 003
   Description "Payment terms code"
   Segment FIELD   PAYMENT_TERMS_CODE  SegType ALPHA  SegOrder ASCENDING
 
Key FAVORITE_ITEM   FOREIGN
   Segment FIELD   FAVORITE_ITEM
 
Relation  1   CUSTOMERS CUSTOMER_NUMBER   ORDERS CUSTOMER_NUMBER
 
Relation  4   CUSTOMERS CUSTOMER_NUMBER   CUSTOMER_EX PRIMARY
 
Relation  2   CUSTOMERS FAVORITE_ITEM   ITEMS ITEM_NUMBER
 
Relation  3   CUSTOMERS CUSTOMER_NUMBER   CUSTOMER_NOTES CUSTOMER_NUMBER
 
Structure CUSTOMER_EX   DBL ISAM
   Description "extended fields for a customer record"
 
Field CUSTOMERID   Type DECIMAL   Size 6
 
Field EXTRADATA   Type ALPHA   Size 128
 
Key PRIMARY   ACCESS   Order ASCENDING   Dups NO
   Segment FIELD   CUSTOMERID
 
Relation  1   CUSTOMER_EX PRIMARY   CUSTOMERS CUSTOMER_NUMBER
 
Structure CUSTOMER_NOTES   DBL ISAM
   Description "Customer notes"
 
Field CUSTOMER_NUMBER   Type DECIMAL   Size 6
   Description "Customer number"
 
Field NOTE_NUMBER   Type DECIMAL   Size 3
   Description "Note number"
 
Field NOTE_TEXT   Type ALPHA   Size 30720
   Description "Note text"
 
Key CUSTOMER_NUMBER   ACCESS   Order ASCENDING   Dups YES   Insert END
   Description "Customer number"
   Segment FIELD   CUSTOMER_NUMBER  SegType DECIMAL  SegOrder ASCENDING
 
Relation  1   CUSTOMER_NOTES CUSTOMER_NUMBER   CUSTOMERS CUSTOMER_NUMBER
 
Structure DATASET   DBL ISAM
   Description "Different Data Types"
 
Field DKEY   Type DECIMAL   Size 18
 
Field IDKEY   Type DECIMAL   Size 28   Precision 10
 
Field AKEY   Type ALPHA   Size 28
 
Field D_1   Type DECIMAL   Size 1
 
Field D_2   Type DECIMAL   Size 2
 
Field D_3   Type DECIMAL   Size 3
 
Field D_4   Type DECIMAL   Size 4
 
Field D_5   Type DECIMAL   Size 5
 
Field D_6   Type DECIMAL   Size 6
 
Field D_7   Type DECIMAL   Size 7
 
Field D_8   Type DECIMAL   Size 8
 
Field D_9   Type DECIMAL   Size 9
 
Field D_10   Type DECIMAL   Size 10   Coerced Type LONG
 
Field D_11   Type DECIMAL   Size 11   Coerced Type LONG
 
Field D_12   Type DECIMAL   Size 12   Coerced Type LONG
 
Field D_13   Type DECIMAL   Size 13   Coerced Type LONG
 
Field D_14   Type DECIMAL   Size 14   Coerced Type LONG
 
Field D_15   Type DECIMAL   Size 15   Coerced Type LONG
 
Field D_16   Type DECIMAL   Size 16   Coerced Type LONG
 
Field D_17   Type DECIMAL   Size 17   Coerced Type LONG
 
Field ID_21   Type DECIMAL   Size 2   Precision 1
 
Field ID_32   Type DECIMAL   Size 3   Precision 2
 
Field ID_42   Type DECIMAL   Size 4   Precision 2
 
Field ID_52   Type DECIMAL   Size 5   Precision 2
 
Field ID_82   Type DECIMAL   Size 8   Precision 2
 
Field ID_84   Type DECIMAL   Size 8   Precision 4
 
Field ID_104   Type DECIMAL   Size 10   Precision 4
 
Field ID_124   Type DECIMAL   Size 12   Precision 4
 
Field ID_186   Type DECIMAL   Size 18   Precision 6
 
Field ID_206   Type DECIMAL   Size 20   Precision 6
 
Field ID_237   Type DECIMAL   Size 23   Precision 7
 
Field ID_268   Type DECIMAL   Size 26   Precision 8
 
Field I_1   Type INTEGER   Size 1
 
Field I_2   Type INTEGER   Size 2
 
Field I_4   Type INTEGER   Size 4
 
Field I_8   Type INTEGER   Size 8
 
Field YYMMDD   Type DATE   Size 6   Stored YYMMDD
 
Field YYYYMMDD   Type DATE   Size 8   Stored YYYYMMDD
 
Field YYJJJ   Type DATE   Size 5   Stored YYJJJ
 
Field YYYYJJJ   Type DATE   Size 7   Stored YYYYJJJ
 
Field YYPP   Type DATE   Size 4   Stored YYPP
 
Field YYYYPP   Type DATE   Size 6   Stored YYYYPP
 
Field MMDDYY   Type USER   Size 6   User Type "^CLASS^=MMDDYY"
   Report Just RIGHT   Input Just RIGHT
 
Field MMDDYYYY   Type USER   Size 8   User Type "^CLASS^=MMDDYYYY"
   Report Just RIGHT   Input Just RIGHT
 
Field DDMMYY   Type USER   Size 6   User Type "^CLASS^=DDMMYY"
   Report Just RIGHT   Input Just RIGHT
 
Field DDMMYYYY   Type USER   Size 8   User Type "^CLASS^=DDMMYYYY"
   Report Just RIGHT   Input Just RIGHT
 
Field JJJJJJ   Type USER   Size 6   User Type "^CLASS^=JJJJJJ"
   Report Just RIGHT   Input Just RIGHT
 
Field JJJYY   Type USER   Size 5   User Type "^CLASS^=JJJYY"
   Report Just RIGHT   Input Just RIGHT
 
Field JJJYYYY   Type USER   Size 7   User Type "^CLASS^=JJJYYYY"
   Report Just RIGHT   Input Just RIGHT
 
Field PPYY   Type USER   Size 4   User Type "^CLASS^=PPYY"
   User Text "^CLASS^=PPYY"   Report Just RIGHT   Input Just RIGHT
 
Field PPYYYY   Type USER   Size 6   User Type "^CLASS^=PPYYYY"
   Report Just RIGHT   Input Just RIGHT
 
Field MONDDYY   Type USER   Size 7   User Type "^CLASS^=MONDDYY"
 
Field MONDDYYYY   Type USER   Size 9   User Type "^CLASS^=MONDDYYYY"
 
Field DDMONYY   Type USER   Size 7   User Type "^CLASS^=DDMONYY"
 
Field DDMONYYYY   Type USER   Size 9   User Type "^CLASS^=DDMONYYYY"
 
Field YYMONDD   Type USER   Size 7   User Type "^CLASS^=YYMONDD"
 
Field YYYYMONDD   Type USER   Size 9   User Type "^CLASS^=YYYYMONDD"
 
Field HHMMSS   Type TIME   Size 6   Stored HHMMSS
   Report Just RIGHT   Input Just RIGHT
 
Field HHMM   Type TIME   Size 4   Stored HHMM
   Report Just RIGHT   Input Just RIGHT
 
Field BINARY10   Type ALPHA   Size 10   Stored BINARY
 
Field BINARY20   Type ALPHA   Size 20   Stored BINARY
 
Field BINARY40   Type ALPHA   Size 40   Stored BINARY
 
Field PACKED6   Type DECIMAL   Size 6
   User Text "^CLASS^=PACKED"
 
Field PACKED8   Type DECIMAL   Size 8
   User Text "^CLASS^=PACKED"
 
Field A1   Type ALPHA   Size 1
 
Field A10   Type ALPHA   Size 10
 
Field A100   Type ALPHA   Size 100
 
Field A255   Type ALPHA   Size 255
 
Field A512   Type ALPHA   Size 512
 
Key DKEY   ACCESS   Order ASCENDING   Dups NO
   Segment FIELD   DKEY
 
Key IDKEY   ACCESS   Order ASCENDING   Dups YES
   Segment FIELD   IDKEY
 
Key AKEY   ACCESS   Order ASCENDING   Dups YES
   Segment FIELD   AKEY
 
Structure DATATABLESTR   DBL ISAM
   Description "DataTable Structure test"
 
Field ACCOUNTNUMBER   Type DECIMAL   Size 6
   Description "Account number"
   Prompt "AccountNo"   Report Heading "AcctNo"
 
Field NAME   Type ALPHA   Size 25
   Description "name"
   Prompt "Name 1"   Report Heading "Name"
 
Field AMOUNT   Type DECIMAL   Size 8   Precision 2
   Prompt "Amountb"   Report Heading "Amounta"
 
Field QTY   Type DECIMAL   Size 5
   Prompt "QtyP"   Info Line "QtyI"   User Text "QtyU"   Report Heading "QtyR"
 
Structure DATESTRU   DBL ISAM
   Description "Date Structure"
 
Field DATE_FIELD   Type DATE   Size 6   Stored YYMMDD
 
Structure DATETEBLESTR2   DBL ISAM
   Description "DateTable structure 2 test with a group"
 
Field ALPHAFLD1   Type ALPHA   Size 10
 
Field DEC4FLD   Type DECIMAL   Size 4
 
Group GROUPFLD   Type ALPHA
 
   Field GPALPHA   Type ALPHA   Size 20
 
   Field GPDEC   Type DECIMAL   Size 5
 
Endgroup
 
Field IDFILED   Type DECIMAL   Size 7   Precision 2
 
Structure DATETIMEARY   DBL ISAM
   Description "Str with date/times as arrays"
 
Field ADTS_DT8   Type DATE   Size 8   Stored YYYYMMDD   Dimension 3
   Description "Date 8 YYYYMMDD"
 
Field ADTS_DT6   Type DATE   Size 6   Stored YYMMDD   Dimension 3
   Description "Date 6 YYMMDD"
 
Field ADTS_DT7   Type DATE   Size 7   Stored YYYYJJJ   Dimension 3
   Description "Date 7 YYYYJJJ"
 
Field ADTS_DT5   Type DATE   Size 5   Stored YYJJJ   Dimension 3
   Description "Date 5 YYJJJ"
 
Field ADTS_TM6   Type TIME   Size 6   Stored HHMMSS   Dimension 3
   Description "Time 6 HHMMSS"
 
Field ADTS_TM4   Type TIME   Size 4   Stored HHMM   Dimension 3
   Description "Time 4 HHMM"
 
Structure DATETIMESTR   DBL ISAM
   Description "To test an array of struct's date/times"
 
Field DTS_ALPHA   Type ALPHA   Size 10
 
Field DTS_DATE8   Type DATE   Size 8   Stored YYYYMMDD
 
Field DTS_DATE6   Type DATE   Size 6   Stored YYMMDD
 
Field DTS_TIME6   Type TIME   Size 6   Stored HHMMSS
 
Field DTS_TIME4   Type TIME   Size 4   Stored HHMM
 
Structure DBAUTHOR   DBL ISAM
   Description "Author file"
 
Field AUTHORID   Type ALPHA   Size 10
   Description "Unique author identifier"
   Report Just RIGHT   Input Just RIGHT
 
Field LAST_NAME   Type ALPHA   Size 20
   Description "Author's last name"
 
Field FIRST_NAME   Type ALPHA   Size 10
   Description "Author's first name"
 
Key AUTHORID   ACCESS   Order ASCENDING   Dups NO
   Segment FIELD   AUTHORID
 
Key LAST_NAME   ACCESS   Order ASCENDING   Dups NO
   Segment FIELD   LAST_NAME
 
Structure DBBAG   DBL ISAM
   Description "Bag simulation"
 
Field BAGID   Type ALPHA   Size 10
   Description "Bag identifier - guest logon"
 
Field BOOKID   Type ALPHA   Size 10
 
Field QTY   Type DECIMAL   Size 2
   Description "Book quantity"
 
Key BAGID   ACCESS   Order ASCENDING   Dups YES
   Segment FIELD   BAGID
 
Structure DBBOOK   DBL ISAM
   Description "Book file"
 
Field BOOKID   Type ALPHA   Size 10
   Description "Unique Book identifier"
 
Field AUTHORID   Type ALPHA   Size 10
 
Field PUBLISHERID   Type ALPHA   Size 10
 
Field TITLE   Type ALPHA   Size 30
   Description "Book's title"
 
Field PAGES   Type INTEGER   Size 4
   Description "Number of pages"
 
Field PRICE   Type DECIMAL   Size 8   Precision 2
 
Field QTY_ON_HAND   Type DECIMAL   Size 4
 
Key BOOKID   ACCESS   Order ASCENDING   Dups NO
   Segment FIELD   BOOKID
 
Key PUBLISHERID   ACCESS   Order ASCENDING   Dups YES
   Segment FIELD   PUBLISHERID
   Segment FIELD   TITLE
 
Key AUTHORID   ACCESS   Order ASCENDING   Dups YES
   Segment FIELD   AUTHORID
   Segment FIELD   TITLE
 
Key TITLE   ACCESS   Order ASCENDING   Dups NO
   Segment FIELD   TITLE
 
Structure DBORDERDTL   DBL ISAM
   Description "Order detail file"
 
Field ORDERNO   Type DECIMAL   Size 10
 
Field BOOKID   Type ALPHA   Size 10
 
Key ORDERNO   ACCESS   Order ASCENDING   Dups YES
   Segment FIELD   ORDERNO
 
Key BOOKID   ACCESS   Order ASCENDING   Dups YES
   Segment FIELD   BOOKID
 
Structure DNETDATETIME   DBL ISAM
   Description ".NET DateTime record structure"
 
Field DTETME14   Type USER   Size 14   Stored DATE
   User Type "^CLASS^=YYYYMMDDHHMMSS"
   Description "The .NET datetime yyyymmddhhmmss"
 
Field DTETME2   Type USER   Size 9   Stored DATE
   User Type "^CLASS^=Monddyyyy"
   Description "user defined date not supported"
 
Field DTETME3   Type USER   Size 14   Stored DATE
   User Type "^CLASS^=yyyymmddhhmmss"
   Description "same date format as field dtetme14"
 
Field DTETME4   Type USER   Size 14   Stored DATE   User Type "YYYYMMDDHHMMSS"
   Description "same as dtetme14 no odbc userdata class"
 
Structure ENUMSTRUCTURE   DBL ISAM
   Description "A structure with two enumerations"
 
Field ID   Type DECIMAL   Size 6
   Description "key"
 
Field DESC   Type ALPHA   Size 30
 
Field ACOLOR   Type ENUM   Size 4   Enum MYCOLOR
 
Field ADAY   Type ENUM   Size 4   Enum DAYOFWEEK
 
Structure FUENTES   DBL ISAM
   Description "Test file for binary array"
 
Field FIELDONE   Type ALPHA   Size 20
 
Field FIELD2   Type DECIMAL   Size 12
 
Field BINARYARRAY   Type ALPHA   Size 6   Stored BINARY
 
Field DEC   Type DECIMAL   Size 5
 
Structure GPC   DBL ISAM
   Description "Primary Structure"
 
Field FLD_1A   Type ALPHA   Size 5
   Description "GPC.FLD_1A"
 
Field FLD_2A   Type ALPHA   Size 7
   Description "GPC.FLD_2A"
 
Group GRP_1A   Type ALPHA
   Description "GPC.GRP_1A"
 
   Field FLD_1B   Type ALPHA   Size 4
      Description "GPC.GRP_1A.FLD_1B"
 
   Field FLD_2B   Type ALPHA   Size 6
      Description "GPC.GRP_1A.FLD_2B"
 
   Field FLD_3B   Type ALPHA   Size 8
      Description "GPC.GRP_1A.FLD_3B"
 
   Group GRP_1B   Type ALPHA
      Description "GPC.GRP_1A.GRP_1B"
 
      Field FLD_1E   Type DECIMAL   Size 2
         Description "GPC.GRP_1A.GRP_1B.FLD_1E"
 
      Field FLD_2E   Type ALPHA   Size 2
         Description "GPC.GRP_1A.GRP_1B.FLD_2E"
 
      Field STRUCT_1E   Type STRUCT   Size 16   Struct GPC3
         Description "GPC.GRP_1A.GRP_1B.STRUCT_1E"
 
      Group STRUCT_2E   Reference GPC4   Type ALPHA
         Description "GPC.GRP_1A.GRP_1B.STRUCT_2E"
 
   Endgroup
 
   Group STRUCT_1B   Reference GPC2   Type ALPHA
      Description "GPC.GRP_1A.STRUCT_1B"
 
Endgroup
 
Field STRUCT_1A   Type STRUCT   Size 41   Struct GPC2
   Description "GPC.STRUCT_1A"
 
Alias AL_GPC   Structure GPC
 
Structure GPC5   DBL ISAM
   Description "Structure with no fields"
 
Alias AL_GPC5   Structure GPC5
 
Structure GPC7   DBL ISAM
   Description "Seventh Structure"
 
Field FIELD_ONE   Type USER   Size 4
 
Group FIELD_TWO   Reference GPC6   Type ALPHA
 
Alias AL_GPC7   Structure GPC7
 
Structure GRFAFILESTRUCT   DBL ISAM
   Description "Structure to fill dummy data file and te"
 
Field FLD1   Type INTEGER   Size 4
 
Field FLD2   Type ALPHA   Size 10
 
Structure GRFATEST   DBL ISAM
   Description "test"
 
Field A_STRING   Type ALPHA   Size 10
 
Field A_GRFA   Type ALPHA   Size 10   Stored BINARY
 
Structure IMPLIEDDECIMALTEST   DBL ISAM
   Description "ID test"
 
Field NAME   Type ALPHA   Size 20
 
Field ID6P6   Type DECIMAL   Size 6   Precision 6
 
Field DESC   Type ALPHA   Size 20
 
Structure INTEGERS   DBL ISAM
   Description "All Integer types"
 
Field MYSBYTE   Type INTEGER   Size 1   Coerced Type SBYTE
 
Field MYBYTE   Type INTEGER   Size 1   Coerced Type BYTE
 
Field MYSHORT   Type INTEGER   Size 2   Coerced Type SHORT
 
Field MYUSHORT   Type INTEGER   Size 2   Coerced Type USHORT
 
Field MYINT   Type INTEGER   Size 4   Coerced Type INT
 
Field MYUINT   Type INTEGER   Size 4   Coerced Type UINT
 
Field MYLONG   Type INTEGER   Size 8   Coerced Type LONG
 
Field MYULONG   Type INTEGER   Size 8   Coerced Type ULONG
 
Field MYDECIMAL   Type DECIMAL   Size 28   Precision 10
   Coerced Type DECIMAL
 
Field MYFLOAT   Type DECIMAL   Size 28   Precision 10
   Coerced Type FLOAT
 
Field MYDOUBLE   Type DECIMAL   Size 28   Precision 10
   Coerced Type DOUBLE
 
Field AMYSBYTE   Type INTEGER   Size 1   Dimension 2
   Coerced Type SBYTE
 
Field AMYBYTE   Type INTEGER   Size 1   Dimension 2
   Coerced Type BYTE
 
Field AMYSHORT   Type INTEGER   Size 2   Dimension 2
   Coerced Type SHORT
 
Field AMYUSHORT   Type INTEGER   Size 2   Dimension 2
   Coerced Type USHORT
 
Field AMYINT   Type INTEGER   Size 4   Dimension 2
   Coerced Type INT
 
Field AMYUINT   Type INTEGER   Size 4   Dimension 2
   Coerced Type UINT
 
Field AMYLONG   Type INTEGER   Size 8   Dimension 2
   Coerced Type LONG
 
Field AMYULONG   Type INTEGER   Size 8   Dimension 2
 
Field AMYFLOAT   Type DECIMAL   Size 28   Precision 10   Dimension 2
   Coerced Type FLOAT
 
Field AMYDOUBLE   Type DECIMAL   Size 28   Precision 10   Dimension 2
   Coerced Type DOUBLE
 
Field AMYDECIMAL   Type DECIMAL   Size 28   Precision 10   Dimension 2
 
Structure INTEGERTESTS   DBL ISAM
   Description "Test for different integer sizes"
 
Field INT_I1   Type INTEGER   Size 1
   Description "Integer size i1"
 
Field INT_I2   Type INTEGER   Size 2
   Description "Integer size i2"
 
Field INT_I4   Type INTEGER   Size 4
   Description "Integer size i4"
 
Field INT_I8   Type INTEGER   Size 8   Coerced Type LONG
   Description "Integer size i8"
 
Structure ITEMS   DBL ISAM
   Description "Item master record"
 
Field ITEM_NUMBER   Type DECIMAL   Size 6
   Description "Item number"
   Long Description
      "SAMPLE_DATA=19;"
   Report Just LEFT
   Required
 
Field VENDOR_NUMBER   Type DECIMAL   Size 6
   Description "Vendor number"
   Long Description
      "SAMPLE_DATA=41;"
   Report Just LEFT   Input Just LEFT
 
Field SIZE   Type DECIMAL   Size 3
   Description "Size in gallons"
   Long Description
      "SAMPLE_DATA=5;"
   Required
   Selection List 1 2 6  Entries "1", "3", "5", "10", "15", "30"
 
Field COMMON_NAME   Type ALPHA   Size 30
   Description "Common name"
   Long Description
      "SAMPLE_DATA=European Hackberry;"
   Required
 
Field LATIN_NAME   Type ALPHA   Size 30
   Description "Latin name"
   Long Description
      "SAMPLE_DATA=Celtis australis;"
   Prompt "Latin name"
 
Field ZONE_CODE   Type DECIMAL   Size 1
   Description "Hardiness zone code"
   Long Description
      "SAMPLE_DATA=2;"
   Break
 
Field TYPE   Type DECIMAL   Size 1
   Description "Type code"
   Long Description
      "SAMPLE_DATA=1;"
   Selection List 1 2 2  Entries "Annual", "Perenn"
   Enumerated 6 1 1
 
Field FLOWERING   Type ALPHA   Size 1
   Description "Flowering?"
   Long Description
      "SAMPLE_DATA=Y;"
 
Field FLOWER_COLOR   Type ALPHA   Size 6
   Description "Flower color"
   Long Description
      "SAMPLE_DATA=Red;"
 
Field SHAPE   Type ALPHA   Size 10
   Description "Shape"
   Long Description
      "SAMPLE_DATA=1;"
   Selection List 1 2 4  Entries "Bush", "Tree", "Vine", "Gcover"
 
Field MAX_HEIGHT   Type DECIMAL   Size 3
   Description "Maximum height (in inches)"
   Long Description
      "SAMPLE_DATA=24;"
 
Field MAX_WIDTH   Type DECIMAL   Size 3
   Description "Maximum width (in inches)"
   Long Description
      "SAMPLE_DATA=30;"
 
Field WATER_REQUIREMENT   Type ALPHA   Size 4
   Description "Water requirements"
   Long Description
      "SAMPLE_DATA=Low;"
   Selection List 1 2 3  Entries "Hig", "Med", "Low"
 
Field SUN_REQUIREMENT   Type ALPHA   Size 6
   Description "Sun requirements"
   Long Description
      "SAMPLE_DATA=Part;"
   Selection List 1 2 4  Entries "Full", "Part", "Shade", "Any"
 
Field BIN_LOCATION   Type ALPHA   Size 3
   Description "Bin/aisle"
   Long Description
      "SAMPLE_DATA=B06;"
 
Field QTY_ON_HAND   Type DECIMAL   Size 6
   Description "Qty on hand"
   Long Description
      "SAMPLE_DATA=17;"
   Negative
 
Field QTY_ALLOCATED   Type DECIMAL   Size 6
   Description "Qty allocated"
   Long Description
      "SAMPLE_DATA=2;"
 
Field QTY_ON_ORDER   Type DECIMAL   Size 6
   Description "Qty on order"
   Long Description
      "SAMPLE_DATA=10;"
 
Field REORDER_LEVEL   Type DECIMAL   Size 6
   Description "Reorder point"
   Long Description
      "SAMPLE_DATA=20;"
 
Field UNIT_PRICE   Type DECIMAL   Size 7   Precision 2
   Description "Unit price"
   Long Description
      "SAMPLE_DATA=15.99;"
 
Field COST_PRICE   Type DECIMAL   Size 7   Precision 2
   Description "Item cost"
   Long Description
      "SAMPLE_DATA=9.99;"
 
Key ITEM_NUMBER   ACCESS   Order ASCENDING   Dups NO
   Segment FIELD   ITEM_NUMBER  SegType DECIMAL
 
Key VENDOR_NUMBER   ACCESS   Order ASCENDING   Dups YES   Insert END
   Modifiable YES
   Segment FIELD   VENDOR_NUMBER  SegType DECIMAL
 
Key COLOR   ACCESS   Order DESCENDING   Dups YES   Insert END   Modifiable YES
   Description "descending a6 key"
   Segment FIELD   FLOWER_COLOR
 
Key SIZE   ACCESS   Order DESCENDING   Dups YES   Insert END   Modifiable YES
   Description "descending decimal key"
   Segment FIELD   SIZE  SegType DECIMAL
 
Key NAME   ACCESS   Order ASCENDING   Dups YES   Insert END   Modifiable YES
   Description "alpha30 ascending key"
   Segment FIELD   COMMON_NAME
 
Relation  1   ITEMS VENDOR_NUMBER   VENDORS VENDOR_NUMBER
 
Relation  2   ITEMS ITEM_NUMBER   ORDER_ITEMS ITEM_ORDERED
 
Structure MANAGE_FUNDS   DBL ISAM
   Description "Structure with large alpha field"
 
Field CUST   Type ALPHA   Size 7
 
Field DPTCDE   Type ALPHA   Size 3
 
Field FNDSTR   Type ALPHA   Size 2000
 
Field ACTION   Type ALPHA   Size 1
 
Field FNDARR   Type ALPHA   Size 20   Dimension 99
 
Structure MSC4   DBL ISAM
   Description "test"
 
Field MSC4_DE5   Type DECIMAL   Size 5
 
Group MSCGRP   Type ALPHA
 
   Field MSC_GRP1_DE3   Type DECIMAL   Size 3
 
   Field MSC_GRP_STRUCT_MSC1   Type STRUCT   Size 15   Struct MSC1
 
   Field MSC_GRP_AL7   Type ALPHA   Size 7
 
Endgroup
 
Field MSC5_AL10   Type ALPHA   Size 10
 
Structure MSC3   DBL ISAM
   Description "test"
 
Field MSC3_DE3   Type DECIMAL   Size 3
 
Field MSC3_STRUCT_MSC2   Type STRUCT   Size 40   Struct MSC2
 
Field MSC3_AL10   Type ALPHA   Size 10
 
Structure NULLDTARY   DBL ISAM
   Description "Str with null date/times as arrays"
 
Field NDTS_DT8   Type DATE   Size 8   Stored YYYYMMDD   Dimension 3
   Coerced Type NULLABLE_DATETIME
   Description "Date 8 YYYYMMDD"
 
Field NDTS_DT6   Type DATE   Size 6   Stored YYMMDD   Dimension 3
   Coerced Type NULLABLE_DATETIME
   Description "Date 6 YYMMDD"
 
Field NDTS_DT7   Type DATE   Size 7   Stored YYYYJJJ   Dimension 3
   Coerced Type NULLABLE_DATETIME
   Description "Date 7 YYYYJJJ"
 
Field NDTS_DT5   Type DATE   Size 5   Stored YYJJJ   Dimension 3
   Coerced Type NULLABLE_DATETIME
   Description "Date 5 YYJJJ"
 
Field NDTS_TM6   Type TIME   Size 6   Stored HHMMSS   Dimension 3
   Coerced Type NULLABLE_DATETIME
   Description "Time 6 HHMMSS"
 
Field NDTS_TM4   Type TIME   Size 4   Stored HHMM   Dimension 3
   Coerced Type NULLABLE_DATETIME
   Description "Time 4 HHMM"
 
Structure NULLDTSTR   DBL ISAM
   Description "To test an array of struct's null date/t"
 
Field NDT_ALPHA   Type ALPHA   Size 10
 
Field NDT_DATE8   Type DATE   Size 8   Stored YYYYMMDD
   Coerced Type NULLABLE_DATETIME
 
Field NDT_DATE6   Type DATE   Size 6   Stored YYMMDD
   Coerced Type NULLABLE_DATETIME
 
Field NDT_TIME6   Type TIME   Size 6   Stored HHMMSS
   Coerced Type NULLABLE_DATETIME
 
Field NDT_TIME4   Type TIME   Size 4   Stored HHMM
   Coerced Type NULLABLE_DATETIME
 
Structure ORDERS   DBL ISAM
   Description "Orders"
 
Field ORDER_NUMBER   Type DECIMAL   Size 6
   Description "Order number"
   Long Description
      "SAMPLE_DATA=162512;"
   Required
 
Field CUSTOMER_NUMBER   Type DECIMAL   Size 6
   Description "Customer number"
   Long Description
      "SAMPLE_DATA=622822;"
   Required
 
Field PLACED_BY   Type ALPHA   Size 25
   Description "Order placed by"
   Long Description
      "SAMPLE_DATA=John Doe;"
   Required
 
Field CUSTOMER_REFERENCE   Type ALPHA   Size 25
   Description "Customer order reference"
   Long Description
      "SAMPLE_DATA=PO12345;"
 
Field PAYMENT_TERMS_CODE   Type ALPHA   Size 2
   Description "Payment terms code"
   Long Description
      "SAMPLE_DATA=30;"
 
Field DATE_ORDERED   Type DATE   Size 8   Stored YYYYMMDD
   Description "Date ordered"
   Long Description
      "SAMPLE_DATA=2018-03-01T00:00:00-08:00;"
   Required
 
Field DATE_COMPLETED   Type DATE   Size 8   Stored YYYYMMDD
   Description "Date order completed"
   Long Description
      "SAMPLE_DATA=2018-03-12T00:00:00-08:00;"
 
Field NONAME_001   Type ALPHA   Size 20   Report Noview   Nonamelink
   Description "Spare space"
   Long Description
      "HARMONY_EXCLUDE"
 
Key ORDER_NUMBER   ACCESS   Order ASCENDING   Dups NO
   Description "Order number"
   Segment FIELD   ORDER_NUMBER  SegType DECIMAL  SegOrder ASCENDING
 
Key CUSTOMER_NUMBER   ACCESS   Order ASCENDING   Dups YES   Insert END
   Modifiable YES   Krf 001
   Description "Customer number"
   Segment FIELD   CUSTOMER_NUMBER  SegType DECIMAL  SegOrder ASCENDING
 
Key DATE_ORDERED   ACCESS   Order ASCENDING   Dups YES   Insert END
   Modifiable YES   Krf 002
   Description "Date ordered"
   Segment FIELD   DATE_ORDERED  SegType DECIMAL  SegOrder ASCENDING
 
Key DATE_COMPLETED   ACCESS   Order ASCENDING   Dups YES   Insert END
   Modifiable YES   Krf 003
   Description "Date order completed"
   Segment FIELD   DATE_COMPLETED  SegType DECIMAL  SegOrder ASCENDING
 
Relation  1   ORDERS ORDER_NUMBER   ORDER_ITEMS ORDER_NUMBER_AND_LINE_ITEM
 
Relation  2   ORDERS CUSTOMER_NUMBER   CUSTOMERS CUSTOMER_NUMBER
 
Structure ORDER_ITEMS   DBL ISAM
   Description "Order items"
 
Field ORDER_NUMBER   Type DECIMAL   Size 6
   Description "Order number"
   Long Description
      "SAMPLE_DATA=5238;"
   Required
 
Field ITEM_NUMBER   Type DECIMAL   Size 2
   Description "Line item number"
   Long Description
      "SAMPLE_DATA=1;"
   Required
 
Field ITEM_ORDERED   Type DECIMAL   Size 6
   Description "Item ordered"
   Long Description
      "SAMPLE_DATA=21;"
   Required
 
Field QUANTITY_ORDERED   Type DECIMAL   Size 6
   Description "Quantity ordered"
   Long Description
      "SAMPLE_DATA=3;"
   Required
 
Field UNIT_PRICE   Type DECIMAL   Size 7   Precision 2
   Description "Unit price"
   Long Description
      "SAMPLE_DATA=15.99;"
   Required
 
Field DATE_SHIPPED   Type DATE   Size 8   Stored YYYYMMDD
   Description "Date shipped"
   Long Description
      "SAMPLE_DATA=2018-03-17T00:00:00-08:00;"
 
Field INVOICE_NUMBER   Type DECIMAL   Size 7
   Description "Invoice number"
   Long Description
      "SAMPLE_DATA=166825;"
 
Field NONAME_001   Type ALPHA   Size 58   Language Noview   Script Noview
   Report Noview   Nonamelink
   Description "Spare space"
 
Key ORDER_NUMBER_AND_LINE_ITEM   ACCESS   Order ASCENDING   Dups NO
   Description "Order number and line number"
   Segment FIELD   ORDER_NUMBER  SegType DECIMAL  SegOrder ASCENDING
   Segment FIELD   ITEM_NUMBER  SegType DECIMAL  SegOrder ASCENDING
 
Key ITEM_ORDERED   ACCESS   Order ASCENDING   Dups YES   Insert END
   Modifiable YES   Krf 001
   Description "Item ordered"
   Segment FIELD   ITEM_ORDERED  SegType DECIMAL  SegOrder ASCENDING
 
Key DATE_SHIPPED   ACCESS   Order ASCENDING   Dups YES   Insert END
   Modifiable YES   Krf 002
   Description "Date item shipped"
   Segment FIELD   DATE_SHIPPED  SegType DECIMAL  SegOrder DESCENDING
 
Key INVOICE_NUMBER   ACCESS   Order ASCENDING   Dups YES   Insert END
   Modifiable YES   Krf 003
   Description "Invoice number billed on"
   Segment FIELD   INVOICE_NUMBER  SegType DECIMAL  SegOrder ASCENDING
 
Relation  1   ORDER_ITEMS ORDER_NUMBER_AND_LINE_ITEM   ORDERS ORDER_NUMBER
 
Relation  2   ORDER_ITEMS ITEM_ORDERED   ITEMS ITEM_NUMBER
 
Structure PERFSTRUCT   DBL ISAM
   Description "performance test structure"
 
Field ALPHA_20   Type ALPHA   Size 20
 
Field DEC_5   Type DECIMAL   Size 5
 
Field DEC_15   Type DECIMAL   Size 15
 
Field IDEC_31   Type DECIMAL   Size 3   Precision 1
 
Field IDEC_144   Type DECIMAL   Size 14   Precision 4
 
Field I_1   Type INTEGER   Size 1
 
Field I_2   Type INTEGER   Size 2
 
Field I_4   Type INTEGER   Size 4
 
Field I_8   Type INTEGER   Size 8
 
Structure PMAUTHOR_RD   DBL ISAM
   Description "Read author parameter"
 
Field AUTHORID   Type ALPHA   Size 10
 
Field LAST_NAME   Type ALPHA   Size 20
 
Field FIRST_NAME   Type ALPHA   Size 10
 
Field BOOKS_WRITTEN   Type INTEGER   Size 4
 
Structure PMBAG_RD   DBL ISAM
   Description "Parameter for reading Nile's bag"
 
Field BOOKID   Type ALPHA   Size 10
 
Field TITLE   Type ALPHA   Size 40
 
Field LAST_NAME   Type ALPHA   Size 20
 
Field PRICE   Type DECIMAL   Size 8   Precision 2
 
Structure PMBOOK_INFO   DBL ISAM
   Description "Nile parameter for book info"
 
Field TITLE   Type ALPHA   Size 40
 
Field LAST_NAME   Type ALPHA   Size 20
 
Field FIRST_NAME   Type ALPHA   Size 10
 
Field PUBLISHER   Type ALPHA   Size 50
 
Field PAGES   Type INTEGER   Size 4
 
Field PRICE   Type DECIMAL   Size 8   Precision 2
 
Field QTY_ON_HAND   Type DECIMAL   Size 4
 
Structure PMBOOK_RD   DBL ISAM
   Description "Read book title parameter"
 
Field BOOKID   Type ALPHA   Size 10
 
Field BOOK_TITLE   Type ALPHA   Size 40
 
Field LAST_NAME   Type ALPHA   Size 20
 
Field NUMBER_PAGES   Type INTEGER   Size 4
 
Field BOOK_PRICE   Type DECIMAL   Size 8   Precision 2
 
Structure PRO_40   DBL ISAM
   Description "Profile test 40 - 20 element alpha"
 
Field A1   Type ALPHA   Size 10
 
Field A2   Type ALPHA   Size 10
 
Field A3   Type ALPHA   Size 10
 
Field A4   Type ALPHA   Size 10
 
Field A5   Type ALPHA   Size 10
 
Field A6   Type ALPHA   Size 10
 
Field A7   Type ALPHA   Size 10
 
Field A8   Type ALPHA   Size 10
 
Field A9   Type ALPHA   Size 10
 
Field A10   Type ALPHA   Size 10
 
Field A11   Type ALPHA   Size 10
 
Field A12   Type ALPHA   Size 10
 
Field A13   Type ALPHA   Size 10
 
Field A14   Type ALPHA   Size 10
 
Field A15   Type ALPHA   Size 10
 
Field A16   Type ALPHA   Size 10
 
Field A17   Type ALPHA   Size 10
 
Field A18   Type ALPHA   Size 10
 
Field A19   Type ALPHA   Size 10
 
Field A20   Type ALPHA   Size 10
 
Structure PRO_41   DBL ISAM
   Description "Profile 41 - 1 field structure array"
 
Field A1   Type ALPHA   Size 10
 
Structure REFCOUNT   DBL ISAM
   Description "reference counting structure test"
 
Field F1   Type ALPHA   Size 10
 
Structure SALESMAN   DBL ISAM
   Description "SALESMAN"
 
Field SALESMANS_CODE   Type ALPHA   Size 3
 
Field SALESMANS_DEPART   Type DECIMAL   Size 2
 
Field SALESMANS_INIT   Type ALPHA   Size 12
 
Field SALESMANS_SURNAME   Type ALPHA   Size 32
 
Field SERVICE_RECEPTIONIST   Type ALPHA   Size 4
 
Field SALESMANS_LASER_QUE   Type ALPHA   Size 10
 
Group SM_TARGET_PER_UNIT_NEW   Type DECIMAL   Size 91   Dimension 15
 
   Field SUB_PER_UNIT_NEW   Type DECIMAL   Size 7   Dimension 13
      Description "2nd dimension of sm_target_per_unit_new"
 
Endgroup
 
Group SM_TARGET_PER_UNIT_USED   Type DECIMAL   Size 91   Dimension 15
 
   Field SUB_PER_UNIT_USED   Type DECIMAL   Size 7   Dimension 13
      Description "2nd array of sm_target_per_unit_used"
 
Endgroup
 
Field SM_TARGET_TYPE   Type ALPHA   Size 3   Dimension 15
 
Field SM_TARGET_UNITS_NEW   Type DECIMAL   Size 3   Dimension 13
 
Field SM_TARGET_UNITS_USED   Type DECIMAL   Size 3   Dimension 13
 
Group SM_TARGET_PENET_PERC_NEW   Type DECIMAL   Size 39   Dimension 15
 
   Field SUB_PENET_PERC_NEW   Type DECIMAL   Size 3   Dimension 13
      Description "2nd dimension of sm_target_penet_perc_nw"
 
Endgroup
 
Group SM_TARGET_PENET_PERC_USED   Type DECIMAL   Size 39   Dimension 15
 
   Field SUB_PENET_PERC_USED   Type DECIMAL   Size 3   Dimension 13
 
Endgroup
 
Field SM_COMM_CALC_FROM   Type DECIMAL   Size 5   Dimension 15
 
Field SM_COMM_CALC_TO   Type DECIMAL   Size 5   Dimension 15
 
Field SM_RATE   Type DECIMAL   Size 3   Dimension 15
 
Field SM_AUGMENTATION   Type DECIMAL   Size 3   Dimension 13
 
Field SM_COMM_EARNED   Type DECIMAL   Size 7   Dimension 12
 
Field SM_RATE_EARNED   Type DECIMAL   Size 3   Dimension 12
 
Field SM_UNITS_FIN   Type DECIMAL   Size 3
 
Field SM_COMMISSION_TYPE   Type ALPHA   Size 1
 
Field SM_USED_DOC_FEE   Type ALPHA   Size 5
 
Field SM_PAYROLL_NUMBER   Type ALPHA   Size 5
 
Field SM_DOC_FEE_PERC   Type ALPHA   Size 3
 
Field SM_COMM_PASSWORD   Type ALPHA   Size 6
 
Field SM_ACCRUAL_ACCOUNT   Type ALPHA   Size 7
 
Field SM_PROFIT_EARNED   Type DECIMAL   Size 8   Dimension 12
 
Field SM_PENSION   Type ALPHA   Size 1
 
Field SM_SOLD_NEW   Type DECIMAL   Size 3   Dimension 12
 
Field SM_SOLD_USED   Type DECIMAL   Size 3   Dimension 12
 
Field SM_ACCRUAL_AMOUNT   Type DECIMAL   Size 8
 
Field SM_ACCRUAL_NIC   Type DECIMAL   Size 8
 
Field SM_ACCRUAL_PEN   Type DECIMAL   Size 8
 
Field SM_DOC_FEE_PAID   Type DECIMAL   Size 8   Dimension 12
 
Field SM_AUGMENTATION_PAID   Type DECIMAL   Size 8   Dimension 12
 
Field SM_DEFAULT_SALE_TYPE   Type ALPHA   Size 1
 
Field SM_PENSION_PER   Type ALPHA   Size 4
 
Field SM_DIARY_TARGETS   Type DECIMAL   Size 3   Dimension 10
 
Field NOT_USED   Type ALPHA   Size 110
 
Key SALESMANS_CODE   ACCESS   Order ASCENDING   Dups NO
   Segment FIELD   SALESMANS_CODE
 
Key SALESMAN_SURNAME   ACCESS   Order ASCENDING   Dups NO
   Segment FIELD   SALESMANS_SURNAME
 
Key SALESMANS_DEPART   ACCESS   Order ASCENDING   Dups NO
   Segment FIELD   SALESMANS_DEPART
 
Key SM_ACCRUAL_ACCOUNT   ACCESS   Order ASCENDING   Dups NO
   Segment FIELD   SM_ACCRUAL_ACCOUNT
 
Structure SINGLEIMPLIEDDECIMAL   DBL ISAM
   Description "single implied decimal"
 
Field ID6P6   Type DECIMAL   Size 6   Precision 6
   Description "ID 6.6"
 
Structure STROPTSOUTTER   DBL ISAM
   Description "Struct Opts outter structure"
 
Field KEY_OUTTER   Type DECIMAL   Size 6
 
Field STR_INSTR   Type STRUCT   Size 39   Struct STROPTSINNER
 
Field DESC_OUTTER   Type ALPHA   Size 25
   Report Just RIGHT   Input Just RIGHT
 
Structure STRTEST1   DBL ISAM
   Description "Str test 1 simple structure"
 
Field STR1_DE6   Type DECIMAL   Size 6
   Description "str test 1 ID field"
 
Field STR1_AL10   Type ALPHA   Size 10
   Description "str t test name"
 
Field STR1_ID62   Type DECIMAL   Size 6   Precision 2
   Description "str 1 test ImpDec 6.2"
 
Field STR1_IN4   Type INTEGER   Size 4
   Description "str test 1 interger 4"
 
Field STR1_AL20   Type ALPHA   Size 20
   Description "Str test 1 Alpha 20"
 
Structure STRTEST12   DBL ISAM
   Description "A structure with an array of structures"
 
Field STR12_AL10   Type ALPHA   Size 10
   Description "Alpha of 10"
 
Group STR12_STRARY   Type ALPHA   Dimension 3
 
   Field STR12_GP1DE5   Type DECIMAL   Size 5
 
   Field STR12_GP1AL8   Type ALPHA   Size 8
 
Endgroup
 
Field STR12_DE7   Type DECIMAL   Size 7
 
Structure STRTEST14   DBL ISAM
   Description "Str with different groups of str in str"
 
Group RECKEY   Type ALPHA
   Description "The record key"
 
   Field COMPANY_CODE   Type DECIMAL   Size 3
      Description "The company code"
 
   Field PROD_ID   Type DECIMAL   Size 7
      Description "Product ID number"
 
Endgroup
 
Field PROD_NAME   Type ALPHA   Size 30
 
Field COST   Type DECIMAL   Size 8   Precision 2
   Description "Item cost"
 
Group LOC_GRP   Type ALPHA
   Description "Location group"
 
   Field WHRS   Type DECIMAL   Size 2
      Description "warerhouse number"
 
   Group ILEGRP   Type ALPHA
 
      Field ROW   Type DECIMAL   Size 3
         Description "row it's on"
 
      Field SHELF   Type DECIMAL   Size 3
         Description "shelf level"
 
   Endgroup
 
   Field COUNT   Type DECIMAL   Size 6
 
Endgroup
 
Field COMMENT   Type ALPHA   Size 50
   Description "comments"
 
Structure STRTEST16   DBL ISAM
   Description "a structure of different types of arrays"
 
Field DECARY   Type DECIMAL   Size 4   Dimension 5
   Description "Decimal array"
 
Field ALPARY   Type ALPHA   Size 10   Dimension 5
   Description "Alpha array"
 
Field IMPARY   Type DECIMAL   Size 6   Precision 2   Dimension 5
   Description "Implied decimal array"
 
Field INTARY   Type INTEGER   Size 4   Dimension 5
   Description "Integer array"
 
Structure STRTEST18   DBL ISAM
   Description "Structure within a str with arrays"
 
Field STR18_AL10   Type ALPHA   Size 10
 
Group STR18_STR1   Type ALPHA
 
   Field STR18_DECARY   Type DECIMAL   Size 4   Dimension 5
      Description "Decimal array"
 
   Field STR18_ALPARY   Type ALPHA   Size 10   Dimension 5
      Description "Alpha array"
 
   Field STR18_IMPARY   Type DECIMAL   Size 6   Precision 2   Dimension 5
 
   Field STR18_INTARY   Type INTEGER   Size 4   Dimension 5
      Description "integer array"
 
Endgroup
 
Field STR18_DE7   Type DECIMAL   Size 7
   Description "Decimal 7 digits"
 
Structure STRTEST19   DBL ISAM
   Description "Str with 2 other str's in it"
 
Field STR19_AL10   Type ALPHA   Size 10
   Description "Alpha 10 field"
 
Group STR19_GRP1   Type ALPHA
 
   Field STR19_GP1DE5   Type DECIMAL   Size 5
      Description "Decimal 5"
 
   Field STR19_GP1AL6   Type ALPHA   Size 6
      Description "alpha 6"
 
Endgroup
 
Group STR19_GRP2   Type ALPHA
   Description "str 2"
 
   Field STR19_GP2AL5   Type ALPHA   Size 5
      Description "alpha 5"
 
   Field STR19_GP2DE5   Type DECIMAL   Size 5
 
   Field STR19_GP2ID5   Type DECIMAL   Size 5   Precision 2
      Description "Implied decimap 5.2"
 
Endgroup
 
Structure STRTEST2   DBL ISAM
   Description "Str test 2 array in a structure"
 
Field STR2_DE10   Type DECIMAL   Size 10
   Description "decimal 10"
 
Field STR2_ARDE5   Type DECIMAL   Size 5   Dimension 3
 
Field STR2_AL20   Type ALPHA   Size 20
 
Field STR2_DATE8   Type DECIMAL   Size 8
   Description "Date field"
   Report Just LEFT   Input Just LEFT
 
Field STR2_TIME6   Type DECIMAL   Size 6
   Description "time field"
   Report Just LEFT   Input Just LEFT
 
Field STR2_ID83   Type DECIMAL   Size 8   Precision 3
 
Field STR2_IN4   Type INTEGER   Size 4
 
Key PRIMARYKEY   ACCESS   Order ASCENDING   Dups NO
   Description "The primary key for the file"
   Segment FIELD   STR2_DE10
   Segment FIELD   STR2_DATE8
 
Key SECONDARYKEY   ACCESS   Order ASCENDING   Dups NO
   Description "A secondary key"
   Segment FIELD   STR2_DE10
 
Structure STRTEST23   DBL ISAM
   Description "nested str with arrays as fields"
 
Group STRGP1   Type ALPHA
   Description "group 1"
 
   Field STR23_DE5   Type DECIMAL   Size 5
      Description "decimal 5"
 
   Field STR23_ALARY   Type ALPHA   Size 5   Dimension 3
 
   Field STR23_DE6   Type DECIMAL   Size 6
 
Endgroup
 
Field STR23_ID5   Type DECIMAL   Size 5   Precision 2
 
Group STRGP2   Type ALPHA
 
   Field STR23_AL10   Type ALPHA   Size 10
 
   Field STR23_DEARY   Type DECIMAL   Size 5   Dimension 3
      Description "Array of de 5"
 
   Field STR23_DE7   Type DECIMAL   Size 7
 
Endgroup
 
Field STR23_AL20   Type ALPHA   Size 20
   Description "alpha 20"
 
Structure STRTEST24   DBL ISAM
   Description "nested str with arrays as fields"
 
Group STRGP1   Type ALPHA
   Description "group 1"
 
   Field STR23_DE5   Type DECIMAL   Size 5
      Description "decimal 5"
 
   Field STR23_ALARY   Type ALPHA   Size 5   Dimension 3
 
   Field STR23_DE6   Type DECIMAL   Size 6
 
Endgroup
 
Field STR23_ID5   Type DECIMAL   Size 5   Precision 2
 
Group STRGP2   Type ALPHA
 
   Field STR23_AL10   Type ALPHA   Size 10
 
   Field STR23_DEARY   Type DECIMAL   Size 5   Dimension 3
      Description "Array of de 5"
 
   Group STRGP3   Type ALPHA
      Description "Group three"
 
      Field STR24_DE4   Type DECIMAL   Size 4
         Description "DEC 4"
 
      Field STR24_ARYDE   Type DECIMAL   Size 4   Dimension 3
 
      Field STR24_AL4   Type ALPHA   Size 4
 
   Endgroup
 
   Field STR23_DE7   Type DECIMAL   Size 7
 
Endgroup
 
Field STR23_AL20   Type ALPHA   Size 20
   Description "alpha 20"
 
Structure STRTEST25   DBL ISAM
   Description "John's test structure"
 
Field STR25_AL10   Type ALPHA   Size 10
   Description "Alpha 10"
 
Group STR25_GRP1   Type ALPHA
   Description "Structure one"
 
   Field STR25_G1AL10   Type ALPHA   Size 10
      Description "Alpha 10"
 
   Field STR25_G1DE8   Type DECIMAL   Size 8
      Description "Dec 8"
 
   Field STR25_IP72   Type DECIMAL   Size 7   Precision 2
      Description "Implied Dec 7.2"
 
   Field STR25_G1IN4   Type INTEGER   Size 4
      Description "Int 4"
 
   Group STR25_GRP2   Type ALPHA
      Description "Group 2"
 
      Field STR25_G2AL10   Type ALPHA   Size 10
         Description "Alpha 10"
 
      Field STR25_G2DE8   Type DECIMAL   Size 8
         Description "Dec 8"
 
   Endgroup
 
Endgroup
 
Field STR25_DE10   Type DECIMAL   Size 10
   Description "Dec 10"
 
Structure STRTEST27   DBL ISAM
   Description "I8 one field structure"
 
Field STR27_I8   Type INTEGER   Size 8   Coerced Type LONG
   Description "Integer 8"
 
Structure STRTEST3   DBL ISAM
   Description "str test 3 str in a str"
 
Field STR3_AL10   Type ALPHA   Size 10
   Description "alpha 10"
   Report Heading "Alpha10"
 
Group STR_GRP1   Type ALPHA
   Description "group 1 in a structure"
 
   Field STR3_GP1AL6   Type ALPHA   Size 6
      Report Heading "Alpha6^field"
 
   Field STR3_GP1DE5   Type DECIMAL   Size 5
      Report Heading "de5 ^ field"
 
   Field STR3_GP1ID52   Type DECIMAL   Size 5   Precision 2
 
Endgroup
 
Field STR3_DE7   Type DECIMAL   Size 7
   Report Heading "DecOf7"
 
Field STR3_AL25   Type ALPHA   Size 25
   Report Heading "Alpha 25"
 
Key PRIMARYKEY   ACCESS   Order ASCENDING   Dups NO
   Description "This is the primary key"
   Segment FIELD   STR3_AL10
   Segment FIELD   STR_GRP1
 
Structure STRTEST30   DBL ISAM
   Description "Structure for an array of 10 test"
 
Field NAME   Type ALPHA   Size 30
   Description "alpha 30"
 
Group ADDRESS   Type ALPHA
 
   Field LINE1   Type ALPHA   Size 30
 
   Field LINE2   Type ALPHA   Size 30
 
   Field CITY   Type ALPHA   Size 30
 
   Field STATE   Type ALPHA   Size 2
 
   Field ZIP   Type ALPHA   Size 5
 
Endgroup
 
Structure STRTEST31   DBL ISAM
   Description "Array str ending with a structure"
 
Field STR31_F1   Type DECIMAL   Size 6
   Description "DE 6"
 
Field STR31_F2   Type DECIMAL   Size 6
   Description "DE 6"
 
Group STR31_GRP   Type ALPHA
 
   Field STR31_F3   Type DECIMAL   Size 6
      Description "de6"
 
   Field STR31_F4   Type DECIMAL   Size 6
      Description "DE 6"
 
Endgroup
 
Structure STRTEST4   DBL ISAM
   Description "Str test 4 (str in str in str) 3 deep"
 
Field STR4_LV1FLD1   Type DECIMAL   Size 10
   Description "DE 10"
 
Field STR4_LV1FLD2   Type ALPHA   Size 20
   Description "AL 20"
 
Group STR4_GRP1   Type ALPHA
 
   Field STR4_LEV2FLD1   Type DECIMAL   Size 6
      Description "DE 6"
 
   Group STR4_LV2GRP2   Type ALPHA
 
      Field STR4_LV3FLD1   Type DECIMAL   Size 12   Precision 2
         Description "ID12.2"
 
      Field STR4_LV3FLD2   Type ALPHA   Size 6
         Description "AL 6"
 
      Field STR4_LV3FLD3   Type DECIMAL   Size 5
         Description "DE 5"
 
   Endgroup
 
   Field STR4_LV2FLD2   Type ALPHA   Size 15
      Description "AL 15"
 
Endgroup
 
Field STR4_LV1FLD3   Type ALPHA   Size 10
   Description "AL 10"
 
Structure STRTEST7   DBL ISAM
   Description "simple structure to test array of ST"
 
Field STR7_AL6   Type ALPHA   Size 6
   Description "Alpha 6 field"
 
Field STR7_DE4   Type DECIMAL   Size 4
   Description "decimal 4"
 
Structure STRUCTURETEST   DBL ISAM
   Description "a test"
 
Field PARM1   Type ALPHA   Size 25
 
Field PARM2   Type DECIMAL   Size 8
 
Field PARM3   Type DECIMAL   Size 8   Precision 3
 
Structure STRU_E   DBL ISAM
   Description "Structure E for testing"
 
Group STRUE_ARRAY   Reference STRU_A   Type ALPHA   Dimension 5
 
Field ALPHA_10   Type ALPHA   Size 10
 
Structure STRU_C   DBL ISAM
   Description "Structure C for testing"
 
Field ALPHA_10   Type ALPHA   Size 10
 
Group STRUC_GRP1   Reference STRU_A   Type ALPHA
 
Field DEC_10   Type DECIMAL   Size 10
 
Key KEY   ACCESS   Order ASCENDING   Dups NO
   Segment FIELD   ALPHA_10
 
Structure STRU_B   DBL ISAM
   Description "Structure B for testing"
 
Field ALPHA_10   Type ALPHA   Size 10
 
Group STRUB_GRP1   Type ALPHA
 
   Field ALPHA_10   Type ALPHA   Size 10
 
   Field DEC_8   Type DECIMAL   Size 8
 
   Field IDEC_72   Type DECIMAL   Size 7   Precision 2
 
   Field INT_4   Type INTEGER   Size 4
 
   Group STRUB_SGRP1   Type ALPHA
 
      Field ALPHA_10   Type ALPHA   Size 10
 
      Field DEC_8   Type DECIMAL   Size 8
 
   Endgroup
 
Endgroup
 
Field DEC_10   Type DECIMAL   Size 10
 
Key KEY   ACCESS   Order ASCENDING   Dups NO
   Segment FIELD   ALPHA_10
 
Structure STRU_D   DBL ISAM
   Description "Structure D for testing"
 
Field ALPHA_10   Type ALPHA   Size 10
 
Field ALPHARAY_10X10   Type ALPHA   Size 10   Dimension 10
 
Field DECRAY_5X6   Type DECIMAL   Size 5   Dimension 6
 
Field IDECRAY_52X4   Type DECIMAL   Size 5   Precision 2   Dimension 4
 
Field INTRAY_2X8   Type INTEGER   Size 2   Dimension 8
 
Structure SYSPARAMS   RELATIVE
   Description "System parameter file"
 
Field PARAM_NAME   Type ALPHA   Size 30
   Description "Parameter name"
 
Field PARAM_VALUE   Type DECIMAL   Size 6
   Description "Parameter value"
 
Key RECORD_NUMBER   ACCESS   Order ASCENDING   Dups NO
   Segment RECORD NUMBER
 
Structure TEST   DBL ISAM
   Description "Test structure"
 
Field ID   Type AUTOSEQ   Size 8
   Description "ID"
   Readonly
   Nonull
 
Field YES_NO   Type ALPHA   Size 1
   Description "Alpha Y/N field (boolean)"
   Checkbox
   Required
 
Key ID   ACCESS   Order ASCENDING   Dups NO
   Description "ID"
   Segment FIELD   ID  SegType SEQUENCE
 
Structure TESTSTRUCT   DBL ISAM
   Description "teststructure"
 
Field TESTBYTE   Type INTEGER   Size 1   Coerced Type BYTE
 
Structure TIMEKEY   DBL ISAM
   Description "Test of the Auto Timestamp key"
 
Field AUTO_TIMESTAMP_FIELD   Type AUTOTIME   Size 8
   Description "This is the key field"
   Readonly
   Nonull
 
Field REMAINDER   Type ALPHA   Size 42
 
Structure TIMESTRU   DBL ISAM
   Description "Time structure"
 
Field TIME_FIELD   Type TIME   Size 4   Stored HHMM
 
Structure TR_SYNINS_INP   DBL ISAM
   Description "Insurance input structure"
 
Field SOFTWARE_AREA   Type ALPHA   Size 255
 
Field DATA_AREA   Type ALPHA   Size 255
 
Field CDATA_AREA   Type ALPHA   Size 255
 
Field WEBSITE_CODE   Type ALPHA   Size 3
 
Field DESTINATION_CODE   Type ALPHA   Size 5
 
Field WEB_PRODUCT_CODE   Type ALPHA   Size 6
 
Field DURATION   Type DECIMAL   Size 3
 
Field INSURANCE_ADULTS   Type DECIMAL   Size 3
 
Field INSURANCE_CHILDREN   Type DECIMAL   Size 3
 
Field INSURANCE_SENIORS   Type DECIMAL   Size 3
 
Field INSURANCE_INFANTS   Type DECIMAL   Size 3
 
Structure TR_SYNINS_RET   DBL ISAM
   Description "Insurance return structure"
 
Field STATUS_CODE   Type ALPHA   Size 3
 
Field SUPPLIER   Type ALPHA   Size 6
 
Field SUPPLIER_NAME   Type ALPHA   Size 30
 
Field ENDORSEMENT   Type ALPHA   Size 4
 
Field ENDORSEMENT_NAME   Type ALPHA   Size 30
 
Field INSURANCE_TYPE   Type ALPHA   Size 2
 
Field INSURANCE_DESCRIPTION   Type ALPHA   Size 10
 
Field INS_DURATION   Type DECIMAL   Size 3
 
Group INS_VALUES   Type ALPHA   Dimension 5
 
   Field PASSENGER_TYPE   Type ALPHA   Size 3
 
   Field GROSS   Type DECIMAL   Size 7
 
   Field NET   Type DECIMAL   Size 7
 
   Field ENDORSEMENT_GROSS   Type DECIMAL   Size 7
 
   Field ENDORSEMENT_NET   Type DECIMAL   Size 7
 
   Field NUMBER_OF_PAX   Type ALPHA   Size 3
      Report Just RIGHT   Input Just RIGHT
 
Endgroup
 
Structure USERDATES   DBL ISAM
   Description "Holds user dates"
 
Field KEYNUMBER   Type DECIMAL   Size 6
 
Field USERDATE14   Type USER   Size 14   Stored DATE
   User Type "YYYYMMDDHHMISS"
 
Field USERDATE20   Type USER   Size 20   Stored DATE
   User Type "YYYYMMDDHHMISSUUUUUU"
 
Structure USERSTRU   DBL ISAM
   Description "User structure"
 
Field USER_FIELD   Type USER   Size 10
 
Structure V93REPOSOPTS   DBL ISAM
   Description "V93 Repoitory Options"
 
Field NEWBOOL   Type BOOLEAN   Size 4
 
Field D2NULL   Type DECIMAL   Size 2   Coerced Type NULLABLE_DECIMAL
   Null Allowed
 
Field D7NULL   Type DECIMAL   Size 7   Coerced Type NULLABLE_DECIMAL
   Null Allowed
 
Field D12NULL   Type DECIMAL   Size 12   Coerced Type DECIMAL
   Null Allowed
 
Field ID72NULL   Type DECIMAL   Size 7   Precision 2
   Coerced Type NULLABLE_DECIMAL
 
Field ID90NULL   Type DECIMAL   Size 9   Coerced Type NULLABLE_DECIMAL
 
Structure VENDORS   DBL ISAM
   Description "Vendor record"
 
Field VENDOR_NUMBER   Type DECIMAL   Size 6
   Description "Vendor number"
   Long Description
      "SAMPLE_DATA=39;"
   Report Just LEFT   Input Just LEFT
   Required
 
Field NAME   Type ALPHA   Size 30
   Description "Vendor name"
   Long Description
      "SAMPLE_DATA=Gardens R Us;"
   Required
 
Field STREET   Type ALPHA   Size 25
   Description "Street address"
   Long Description
      "SAMPLE_DATA=97 Main St;"
 
Field CITY   Type ALPHA   Size 20
   Description "City"
   Long Description
      "SAMPLE_DATA=Concord;"
 
Field STATE   Type ALPHA   Size 2
   Description "State"
   Long Description
      "SAMPLE_DATA=NH;"
   Uppercase
 
Field ZIP_CODE   Type DECIMAL   Size 5
   Description "Zip Code"
   Long Description
      "SAMPLE_DATA=03214;"
 
Field CONTACT   Type ALPHA   Size 25
   Description "Contact name"
   Long Description
      "SAMPLE_DATA=Jeremiah Johnson;"
 
Field PHONE   Template PHONE
   Description "Phone number"
   Long Description
      "SAMPLE_DATA=(555) 627-2663;"
   Report Heading "Telephone"
 
Field FAX   Template PHONE
   Description "Fax number"
   Long Description
      "SAMPLE_DATA=(555) 627-6382;"
 
Field PAYMENT_TERMS_CODE   Type ALPHA   Size 2
   Description "Payment terms code"
   Long Description
      "SAMPLE_DATA=60;"
   Selection List 0 0 0  Entries "CA", "30", "60", "90"
 
Key VENDOR_NUMBER   ACCESS   Order ASCENDING   Dups NO
   Segment FIELD   VENDOR_NUMBER  SegType DECIMAL
 
Key STATE   ACCESS   Order ASCENDING   Dups YES   Insert END   Modifiable YES
   Krf 002
   Description "State"
   Segment FIELD   STATE  SegType ALPHA  SegOrder ASCENDING
 
Key ZIP   ACCESS   Order ASCENDING   Dups YES   Insert END   Modifiable YES
   Description "zip"
   Segment FIELD   ZIP_CODE  SegType DECIMAL  SegOrder ASCENDING
 
Key PAYMENT_TERMS   ACCESS   Order ASCENDING   Dups YES   Insert END
   Modifiable YES
   Description "Payment terms code"
   Segment FIELD   PAYMENT_TERMS_CODE  SegType ALPHA  SegOrder ASCENDING
 
Relation  1   VENDORS VENDOR_NUMBER   ITEMS VENDOR_NUMBER
 
File AUTH   DBL ISAM   "DATA:auth"
   Description "Authors"
   Assign DBAUTHOR
 
File BAG   DBL ISAM   "DATA:bag"
   Description "Bag of items"
   Assign DBBAG
 
File BOOK   DBL ISAM   "DATA:book"
   Description "Books"
   Assign DBBOOK
 
File CUSTOMERS   DBL ISAM   "DAT:customers.ism"
   Description "Customer master file"
   Assign CUSTOMERS
 
File CUSTOMER_EX   DBL ISAM   "DAT:CUSTOMER_EX.ism"
   Description "extended customer data"
   RecType VARIABLE   Compress   Terabyte   Stored GRFA
   Assign CUSTOMER_EX
 
File CUSTOMER_NOTES   DBL ISAM   "DAT:customer_notes.ism"
   Description "Customer notes file"
   Density 50   Addressing 40BIT   Compress   Static RFA   Terabyte
   Stored GRFA
   Assign CUSTOMER_NOTES
 
File ITEMS   DBL ISAM   "DAT:items.ism"
   Description "Inventory master file"
   Assign ITEMS
 
File ORD   DBL ISAM   "DATA:ord"
   Description "Order header"
   Assign DBORDER
 
File ORDDTL   DBL ISAM   "DATA:orddtl"
   Description "Order detail"
   Assign DBORDERDTL
 
File ORDERS   DBL ISAM   "DAT:orders.ism"
   Description "Order header file"
   Assign ORDERS
 
File ORDER_ITEMS   DBL ISAM   "DAT:order_items.ism"
   Description "Order items file"
   Assign ORDER_ITEMS
 
File PUB   DBL ISAM   "DATA:pub"
   Description "Publishers"
   Assign DBPUBLISHER
 
File SYSPARAMS   RELATIVE   "DAT:sysparams.ddf"
   Description "System parameter file"
   Assign SYSPARAMS
 
File TEST   DBL ISAM   "DAT:test.ism"
   Description "Test file"
   Assign TEST
 
File VENDORS   DBL ISAM   "DAT:vendors.ism"
   Description "Vendors file"
   Assign VENDORS
 
