 
;  SYNERGY DATA LANGUAGE OUTPUT
;
;  REPOSITORY     : D:\HarmonyCore\HarmonyCore.Test.Repository\bin\Debug\rpsmain
;                 : D:\HarmonyCore\HarmonyCore.Test.Repository\bin\Debug\rpstext
;                 : Version 11.0.1
;
;  GENERATED      : 17-AUG-2018, 22:19:15
;                 : Version 10.3.3e
;  EXPORT OPTIONS : [ALL] 
 
 
Format PHONE   Type NUMERIC   "(XXX) XXX-XXXX"   Justify RIGHT
 
Enumeration METHOD_STATUS
   Description "Method return status code"
   Members SUCCESS 0, WARNING 1, ERROR 2, FATAL 3
 
Template DATE   Type DATE   Size 8   Stored YYYYMMDD
   Description "YYYYMMDD date"
   Prompt "Date"   Report Heading "Date"   ODBC Name DATE
 
Template PHONE   Type DECIMAL   Size 10
   Description "Phone number"
   Prompt "Phone #:"   User Text "HARMONY_AS_STRING"   Format PHONE
   Report Heading "Phone Number"
 
Structure CUSTOMERS   DBL ISAM
   Description "Customer record"
 
Tag FIELD   RECORD_TYPE EQ "0"
 
Field CUSTOMER_NUMBER   Type DECIMAL   Size 6
   Description "Customer number"
   Long Description
      "<SAMPLE_DATA>355232</SAMPLE_DATA>"
   Report Just LEFT   Input Just LEFT
 
Field RECORD_TYPE   Type DECIMAL   Size 1
   Description "Record type (0)"
   Long Description
      "<SAMPLE_DATA>0</SAMPLE_DATA>"
 
Field NAME   Type ALPHA   Size 30
   Description "Customer name"
   Long Description
      "<SAMPLE_DATA>Abe's Nursery</SAMPLE_DATA>"
 
Field STREET   Type ALPHA   Size 25
   Description "Street address"
   Long Description
      "<SAMPLE_DATA>1032 Main Street</SAMPLE_DATA>"
 
Field CITY   Type ALPHA   Size 20
   Description "City"
   Long Description
      "<SAMPLE_DATA>Springfield</SAMPLE_DATA>"
 
Field STATE   Type ALPHA   Size 2
   Description "State"
   Long Description
      "<SAMPLE_DATA>MO</SAMPLE_DATA>"
 
Field ZIP_CODE   Type DECIMAL   Size 9
   Description "Zip code"
   Long Description
      "<SAMPLE_DATA>64127</SAMPLE_DATA>"
   Report Just LEFT   Input Just LEFT
 
Field CONTACT   Type ALPHA   Size 25
   Description "Contact name"
   Long Description
      "<SAMPLE_DATA>Abe Albright</SAMPLE_DATA>"
 
Field PHONE   Template PHONE
   Description "Phone number"
   Long Description
      "<SAMPLE_DATA>(555) 123-4567</SAMPLE_DATA>"
 
Field FAX   Template PHONE
   Description "Fax number"
   Long Description
      "<SAMPLE_DATA>(555) 987-6543</SAMPLE_DATA>"
 
Field FAVORITE_ITEM   Type DECIMAL   Size 6
   Description "Customers favorite item"
   Long Description
      "<SAMPLE_DATA>7</SAMPLE_DATA>"
   Report Just LEFT   Input Just LEFT
 
Field PAYMENT_TERMS_CODE   Type ALPHA   Size 2
   Description "Payment terms code"
   Long Description
      "<SAMPLE_DATA>30</SAMPLE_DATA>"
 
Field TAX_ID   Type DECIMAL   Size 9
   Description "Customers tax ID number"
   Long Description
      "<SAMPLE_DATA>546874521</SAMPLE_DATA>"
 
Field CREDIT_LIMIT   Type DECIMAL   Size 7   Precision 2
   Description "Credit limit"
   Long Description
      "<SAMPLE_DATA>5000</SAMPLE_DATA>"
 
Key CUSTOMER_NUMBER   ACCESS   Order ASCENDING   Dups NO
   Segment FIELD   CUSTOMER_NUMBER  SegType DECIMAL
 
Key TAG_CUSTOMER   ACCESS   Order ASCENDING   Dups NO
   Description "Record type (0) and customer number"
   Segment FIELD   RECORD_TYPE  SegType DECIMAL
   Segment FIELD   CUSTOMER_NUMBER  SegType DECIMAL
 
Key FAVORITE_ITEM   FOREIGN
   Segment FIELD   FAVORITE_ITEM
 
Relation  2   CUSTOMERS CUSTOMER_NUMBER   ORDERS CUSTOMER_NUMBER
 
Relation  3   CUSTOMERS FAVORITE_ITEM   ITEMS ITEM_NUMBER
 
Structure ORDERS   DBL ISAM
   Description "Orders"
 
Field ORDER_NUMBER   Type DECIMAL   Size 6
   Description "Order number"
   Long Description
      "<SAMPLE_DATA>162512</SAMPLE_DATA>"
   Required
 
Field CUSTOMER_NUMBER   Type DECIMAL   Size 6
   Description "Customer number"
   Long Description
      "<SAMPLE_DATA>622822</SAMPLE_DATA>"
   Required
 
Field PLACED_BY   Type ALPHA   Size 25
   Description "Order placed by"
   Long Description
      "<SAMPLE_DATA>John Doe</SAMPLE_DATA>"
   Required
 
Field CUSTOMER_REFERENCE   Type ALPHA   Size 25
   Description "Customer order reference"
   Long Description
      "<SAMPLE_DATA>PO12345</SAMPLE_DATA>"
 
Field PAYMENT_TERMS_CODE   Type ALPHA   Size 2
   Description "Payment terms code"
   Long Description
      "<SAMPLE_DATA>30</SAMPLE_DATA>"
 
Field DATE_ORDERED   Type DATE   Size 8   Stored YYYYMMDD
   Description "Date ordered"
   Long Description
      "<SAMPLE_DATA>2018-03-01T00:00:00-08:00</SAMPLE_DATA>"
   Required
 
Field DATE_COMPLETED   Type DATE   Size 8   Stored YYYYMMDD
   Description "Date order completed"
   Long Description
      "<SAMPLE_DATA>2018-03-12T00:00:00-08:00</SAMPLE_DATA>"
 
Field NONAME_001   Type ALPHA   Size 20
   Report Noview   Nonamelink
   Description "Spare space"
 
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
      "<SAMPLE_DATA>5238</SAMPLE_DATA>"
   Required
 
Field ITEM_NUMBER   Type DECIMAL   Size 2
   Description "Line item number"
   Long Description
      "<SAMPLE_DATA>1</SAMPLE_DATA>"
   Required
 
Field ITEM_ORDERED   Type DECIMAL   Size 6
   Description "Item ordered"
   Long Description
      "<SAMPLE_DATA>21</SAMPLE_DATA>"
   Required
 
Field QUANTITY_ORDERED   Type DECIMAL   Size 6
   Description "Quantity ordered"
   Long Description
      "<SAMPLE_DATA>3</SAMPLE_DATA>"
   Required
 
Field UNIT_PRICE   Type DECIMAL   Size 7   Precision 2
   Description "Unit price"
   Long Description
      "<SAMPLE_DATA>15.99</SAMPLE_DATA>"
   Required
 
Field DATE_SHIPPED   Type DATE   Size 8   Stored YYYYMMDD
   Description "Date shipped"
   Long Description
      "<SAMPLE_DATA>2018-03-17T00:00:00-08:00</SAMPLE_DATA>"
 
Field INVOICE_NUMBER   Type DECIMAL   Size 7
   Description "Invoice number"
   Long Description
      "<SAMPLE_DATA>166825</SAMPLE_DATA>"
 
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
 
Structure ITEMS   DBL ISAM
   Description "Item master record"
 
Field ITEM_NUMBER   Type DECIMAL   Size 6
   Description "Item number"
   Long Description
      "<SAMPLE_DATA>19</SAMPLE_DATA>"
   Report Just LEFT
   Required
 
Field VENDOR_NUMBER   Type DECIMAL   Size 6
   Description "Vendor number"
   Long Description
      "<SAMPLE_DATA>41</SAMPLE_DATA>"
   Report Just LEFT   Input Just LEFT
 
Field SIZE   Type DECIMAL   Size 3
   Description "Size in gallons"
   Long Description
      "<SAMPLE_DATA>5</SAMPLE_DATA>"
   Required
   Selection List 1 2 6  Entries "1", "3", "5", "10", "15", "30"
 
Field COMMON_NAME   Type ALPHA   Size 30
   Description "Common name"
   Long Description
      "<SAMPLE_DATA>European Hackberry</SAMPLE_DATA>"
 
Field LATIN_NAME   Type ALPHA   Size 30
   Description "Latin name"
   Long Description
      "<SAMPLE_DATA>Celtis australis</SAMPLE_DATA>"
   Prompt "Latin name"
 
Field ZONE_CODE   Type DECIMAL   Size 1
   Description "Hardiness zone code"
   Long Description
      "<SAMPLE_DATA>2</SAMPLE_DATA>"
   Break
 
Field TYPE   Type DECIMAL   Size 1
   Description "Type code"
   Long Description
      "<SAMPLE_DATA>1</SAMPLE_DATA>"
   Selection List 1 2 2  Entries "Annual", "Perenn"
   Enumerated 6 1 1
 
Field FLOWERING   Type ALPHA   Size 1
   Description "Flowering?"
   Long Description
      "<SAMPLE_DATA>Y</SAMPLE_DATA>"
 
Field FLOWER_COLOR   Type ALPHA   Size 6
   Description "Flower color"
   Long Description
      "<SAMPLE_DATA>Red</SAMPLE_DATA>"
 
Field SHAPE   Type ALPHA   Size 10
   Description "Shape"
   Long Description
      "<SAMPLE_DATA>1</SAMPLE_DATA>"
   Selection List 1 2 4  Entries "Bush", "Tree", "Vine", "Gcover"
 
Field MAX_HEIGHT   Type DECIMAL   Size 3
   Description "Maximum height (in inches)"
   Long Description
      "<SAMPLE_DATA>24</SAMPLE_DATA>"
 
Field MAX_WIDTH   Type DECIMAL   Size 3
   Description "Maximum width (in inches)"
   Long Description
      "<SAMPLE_DATA>30</SAMPLE_DATA>"
 
Field WATER_REQUIREMENT   Type ALPHA   Size 4
   Description "Water requirements"
   Long Description
      "<SAMPLE_DATA>Low</SAMPLE_DATA>"
   Selection List 1 2 3  Entries "Hig", "Med", "Low"
 
Field SUN_REQUIREMENT   Type ALPHA   Size 6
   Description "Sun requirements"
   Long Description
      "<SAMPLE_DATA>Part</SAMPLE_DATA>"
   Selection List 1 2 4  Entries "Full", "Part", "Shade", "Any"
 
Field BIN_LOCATION   Type ALPHA   Size 3
   Description "Bin/aisle"
   Long Description
      "<SAMPLE_DATA>B06</SAMPLE_DATA>"
 
Field QTY_ON_HAND   Type DECIMAL   Size 6
   Description "Qty on hand"
   Long Description
      "<SAMPLE_DATA>17</SAMPLE_DATA>"
   Negative
 
Field QTY_ALLOCATED   Type DECIMAL   Size 6
   Description "Qty allocated"
   Long Description
      "<SAMPLE_DATA>2</SAMPLE_DATA>"
 
Field QTY_ON_ORDER   Type DECIMAL   Size 6
   Description "Qty on order"
   Long Description
      "<SAMPLE_DATA>10</SAMPLE_DATA>"
 
Field REORDER_LEVEL   Type DECIMAL   Size 6
   Description "Reorder point"
   Long Description
      "<SAMPLE_DATA>20</SAMPLE_DATA>"
 
Field UNIT_PRICE   Type DECIMAL   Size 7   Precision 2
   Description "Unit price"
   Long Description
      "<SAMPLE_DATA>15.99</SAMPLE_DATA>"
 
Field COST_PRICE   Type DECIMAL   Size 7   Precision 2
   Description "Item cost"
   Long Description
      "<SAMPLE_DATA>9.99</SAMPLE_DATA>"
 
Key ITEM_NUMBER   ACCESS   Order ASCENDING   Dups NO
   Segment FIELD   ITEM_NUMBER  SegType DECIMAL
 
Key VENDOR_NUMBER   ACCESS   Order ASCENDING   Dups YES   Insert END   Modifiable YES
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
 
Structure VENDORS   DBL ISAM
   Description "Vendor record"
 
Tag FIELD   RECORD_TYPE EQ "1"
 
Field VENDOR_NUMBER   Type DECIMAL   Size 6
   Description "Vendor number"
   Long Description
      "<SAMPLE_DATA>39</SAMPLE_DATA>"
   Report Just LEFT   Input Just LEFT
 
Field RECORD_TYPE   Type DECIMAL   Size 1
   Description "Record type (1)"
   Long Description
      "<SAMPLE_DATA>1</SAMPLE_DATA>"
   Report Just LEFT   Input Just LEFT
 
Field NAME   Type ALPHA   Size 30
   Description "Vendor name"
   Long Description
      "<SAMPLE_DATA>Gardens R Us</SAMPLE_DATA>"
 
Field STREET   Type ALPHA   Size 25
   Description "Street address"
   Long Description
      "<SAMPLE_DATA>97 Main St</SAMPLE_DATA>"
 
Field CITY   Type ALPHA   Size 20
   Description "City"
   Long Description
      "<SAMPLE_DATA>Concord</SAMPLE_DATA>"
 
Field STATE   Type ALPHA   Size 2
   Description "State"
   Long Description
      "<SAMPLE_DATA>NH</SAMPLE_DATA>"
 
Field ZIP_CODE   Type DECIMAL   Size 9
   Description "Zip Code"
   Long Description
      "<SAMPLE_DATA>03214</SAMPLE_DATA>"
 
Field CONTACT   Type ALPHA   Size 25
   Description "Contact name"
   Long Description
      "<SAMPLE_DATA>Jeremiah Johnson</SAMPLE_DATA>"
 
Field PHONE   Template PHONE
   Description "Phone number"
   Long Description
      "<SAMPLE_DATA>(555) 627-2663</SAMPLE_DATA>"
   Report Heading "Telephone"
 
Field FAX   Template PHONE
   Description "Fax number"
   Long Description
      "<SAMPLE_DATA>(555) 627-6382</SAMPLE_DATA>"
 
Field PAYMENT_TERMS_CODE   Type ALPHA   Size 24
   Description "Payment terms code"
   Long Description
      "<SAMPLE_DATA>60</SAMPLE_DATA>"
 
Key VENDOR_NUMBER   ACCESS   Order ASCENDING   Dups NO
   Segment FIELD   VENDOR_NUMBER  SegType DECIMAL
 
Key TAG_VENDOR   ACCESS   Order ASCENDING   Dups NO   Modifiable YES
   Description "Record type (1) and vendor number"
   Segment FIELD   RECORD_TYPE  SegType DECIMAL
   Segment FIELD   VENDOR_NUMBER  SegType DECIMAL
 
Relation  1   VENDORS VENDOR_NUMBER   ITEMS VENDOR_NUMBER
 
File CUSTOMER   DBL ISAM   "ICSTUT:customer.ism"
   Description "Customers and vendors file"
   Assign CUSTOMERS, VENDORS
 
File ORDERS   DBL ISAM   "ICSTUT:orders.ism"
   Description "Order file"
   Assign ORDERS
 
File ORDER_ITEMS   DBL ISAM   "ICSTUT:order_items.ism"
   Description "Order items file"
   Assign ORDER_ITEMS
 
File ITEMS   DBL ISAM   "ICSTUT:ITEMS.ism"
   Description "Inventory File"
   Assign ITEMS
 
