 
;  SYNERGY DATA LANGUAGE OUTPUT
;
;  REPOSITORY     : C:\DEV\SYNERGEX\HarmonyWebServices\HarmonyCore.Test.Reposito
;                 : C:\DEV\SYNERGEX\HarmonyWebServices\HarmonyCore.Test.Reposito
;                 : Version 10.3.4
;
;  GENERATED      : 15-JUN-2018, 11:44:28
;                 : Version 10.3.4
;  EXPORT OPTIONS : [ALL] 
 
 
Format PHONE   Type NUMERIC   "(ZZZ) ZZZ-ZZZZ"   Justify RIGHT
 
Template DATE   Type DATE   Size 8   Stored YYYYMMDD
   Description "YYYYMMDD date"
   Long Description
      "Date in CCYYMMDD format"
   Prompt "Date"   Report Heading "Date"   ODBC Name DATE
 
Template PHONE   Type DECIMAL   Size 10
   Description "Phone number (American)"
   Long Description
      "Telephone number"
   Prompt "Phone #:"   Format PHONE   Report Heading "Phone Number"
 
Structure CUSTOMERS   DBL ISAM
   Description "Customer Record"
 
Tag FIELD   CUST_RTYPE EQ "0"
 
Field CUST_KEY   Type DECIMAL   Size 6
   Description "Customer id"
   Report Just LEFT   Input Just LEFT
 
Field CUST_RTYPE   Type DECIMAL   Size 1
   Description "Record Tag Field"
 
Field CUST_NAME   Type ALPHA   Size 30
   Description "Customer Name"
 
Field CUST_STREET   Type ALPHA   Size 25
   Description "Street Address"
 
Field CUST_CITY   Type ALPHA   Size 20
   Description "City"
 
Field CUST_STATE   Type ALPHA   Size 2
   Description "State"
 
Field CUST_ZIP   Type DECIMAL   Size 9
   Description "Zip code"
   Report Just LEFT   Input Just LEFT
 
Field CUST_CONTACT   Type ALPHA   Size 25
   Description "Contact"
 
Field CUST_PHONE   Template PHONE
   Description "Phone number"
   NoODBC Name
 
Field CUST_FAX   Template PHONE
   Description "FAX number"
   NoODBC Name
 
Field CUST_GIFT   Type DECIMAL   Size 6
   Description "Customer Premium Gift"
   Report Just LEFT   Input Just LEFT
 
Field CUST_TCODE   Type ALPHA   Size 2
   Description "Terms code"
 
Field CUST_TAXNO   Type DECIMAL   Size 9
   Description "Customer Tax Number"
 
Field CUST_LIMIT   Type DECIMAL   Size 7   Precision 2
   Description "Credit limit"
 
Key CUSTOMER   ACCESS   Order ASCENDING   Dups NO
   Segment FIELD   CUST_KEY
 
Key TAG_CUSTOMER   ACCESS   Order ASCENDING   Dups NO
   Description "access key of tag+key"
   Segment FIELD   CUST_RTYPE  SegType ALPHA
   Segment FIELD   CUST_KEY  SegType ALPHA
 
Key ITEM   FOREIGN
   Segment FIELD   CUST_GIFT
 
Relation  2   CUSTOMERS CUSTOMER   ORDERS CUSTOMER
 
Relation  3   CUSTOMERS ITEM   PLANTS ITEM
 
Structure ORDERS   DBL ISAM
   Description "Open Orders Record"
 
Field OR_NUMBER   Type DECIMAL   Size 6
   Description "Order number"
 
Field OR_VENDOR   Type DECIMAL   Size 6
   Description "Vendorcode"
   Report Just LEFT   Input Just LEFT
 
Field OR_ITEM   Type DECIMAL   Size 6
   Description "Item code"
   Report Just LEFT   Input Just LEFT
 
Field OR_CUSTOMER   Type DECIMAL   Size 6
   Description "Customer number"
   Report Just LEFT   Input Just LEFT
 
Field OR_QTY   Type DECIMAL   Size 6
   Description "Quantity on order"
 
Field OR_PRICE   Type DECIMAL   Size 7   Precision 2
   Description "Order Price"
 
Field OR_TERMS   Type ALPHA   Size 2
   Description "Order Terms code"
 
Field OR_ODATE   Template DATE
   Description "Order date"
   NoODBC Name
 
Field OR_SDATE   Template DATE
   Description "Req ship date"
   NoODBC Name
 
Field OR_EDATE   Template DATE
   Description "Expiration date"
   NoODBC Name
 
Field OR_INVOICE   Type DECIMAL   Size 7
   Description "Invoice Number"
 
Key ORDER   ACCESS   Order ASCENDING   Dups NO
   Segment FIELD   OR_NUMBER
 
Key VENDOR   ACCESS   Order ASCENDING   Dups YES   Insert END   Modifiable YES
   Segment FIELD   OR_VENDOR
 
Key ITEM   ACCESS   Order ASCENDING   Dups YES   Insert END   Modifiable YES
   Segment FIELD   OR_ITEM
 
Key CUSTOMER   ACCESS   Order ASCENDING   Dups YES   Insert END
   Modifiable YES
   Segment FIELD   OR_CUSTOMER
 
Key TAG_VENDOR   FOREIGN
   Description "foreign key to vendors (tag + key)"
   Segment LITERAL   "1"
   Segment FIELD   OR_VENDOR
 
Key TAG_CUSTOMER   FOREIGN
   Description "foreign key to customers (tag + key)"
   Segment LITERAL   "0"
   Segment FIELD   OR_CUSTOMER
 
Relation  1   ORDERS VENDOR   VENDORS VENDOR
 
Relation  3   ORDERS CUSTOMER   CUSTOMERS CUSTOMER
 
Relation  4   ORDERS ITEM   PLANTS ITEM
 
Relation  5   ORDERS TAG_CUSTOMER   CUSTOMERS TAG_CUSTOMER
 
Relation  6   ORDERS TAG_VENDOR   VENDORS TAG_VENDOR
 
Structure PLANTS   DBL ISAM
   Description "Plant Master Record"
 
Field IN_ITEMID   Type DECIMAL   Size 6
   Description "Item ID"
   Report Just LEFT
   Required
 
Field IN_SOURCE   Type DECIMAL   Size 6
   Description "Vending Source Code"
   Report Just LEFT   Input Just LEFT
 
Field IN_SIZE   Type DECIMAL   Size 3
   Description "Size in gallons"
   Required
   Selection List 1 2 6  Entries "1", "3", "5", "10", "15", "30"
 
Field IN_NAME   Type ALPHA   Size 30
   Description "Common name"
 
Field IN_LATIN   Type ALPHA   Size 30
   Description "Latin name"
   Prompt "Latin name"
 
Field IN_ZONE   Type DECIMAL   Size 1
   Description "Zone"
   Break
 
Field IN_TYPE   Type DECIMAL   Size 1
   Description "Type code"
   Selection List 1 2 2  Entries "Annual", "Perenn"
   Enumerated 6 1 1
 
Field IN_FLOWER   Type ALPHA   Size 1
   Description "Flowering?"
 
Field IN_COLOR   Type ALPHA   Size 6
   Description "Flower color"
 
Field IN_SHAPE   Type ALPHA   Size 10
   Description "Plant shape"
   Selection List 1 2 4  Entries "Bush", "Tree", "Vine", "Gcover"
 
Field IN_MAXHIGH   Type DECIMAL   Size 3
   Description "Maximum height (in inches)"
 
Field IN_MAXWIDE   Type DECIMAL   Size 3
   Description "Maximum width (in inches)"
 
Field IN_WATER   Type ALPHA   Size 4
   Description "Water requirements"
   Selection List 1 2 3  Entries "Hig", "Med", "Low"
 
Field IN_SUN   Type ALPHA   Size 6
   Description "Sun requirements"
   Selection List 1 2 4  Entries "Full", "Part", "Shade", "Any"
 
Field IN_LOCATION   Type ALPHA   Size 3
   Description "Bin/aisle"
 
Field IN_ONHAND   Type DECIMAL   Size 6
   Description "Qty on hand"
   Negative
 
Field IN_ALLOCATED   Type DECIMAL   Size 6
   Description "Qty allocated"
 
Field IN_ONORDER   Type DECIMAL   Size 6
   Description "Qty on order"
 
Field IN_MINIMUM   Type DECIMAL   Size 6
   Description "Reorder point"
 
Field IN_PRICE   Type DECIMAL   Size 7   Precision 2
   Description "Unit Price"
 
Field IN_COST   Type DECIMAL   Size 7   Precision 2
   Description "Item Cost"
 
Key ITEM   ACCESS   Order ASCENDING   Dups NO
   Segment FIELD   IN_ITEMID  SegType DECIMAL
 
Key VENDOR   ACCESS   Order ASCENDING   Dups YES   Insert END   Modifiable YES
   Segment FIELD   IN_SOURCE  SegType ALPHA
 
Key COLOR   ACCESS   Order DESCENDING   Dups YES   Insert END   Modifiable YES
   Description "descending a6 key"
   Segment FIELD   IN_COLOR
 
Key SIZE   ACCESS   Order DESCENDING   Dups YES   Insert END   Modifiable YES
   Description "descending decimal key"
   Segment FIELD   IN_SIZE  SegType ALPHA
 
Key NAME   ACCESS   Order ASCENDING   Dups YES   Insert END   Modifiable YES
   Description "alpha30 ascending key"
   Segment FIELD   IN_NAME
 
Key REVERSE_VENDOR   ACCESS   Order DESCENDING   Dups YES   Insert END
   Modifiable YES
   Description "in_source in descending order"
   Segment FIELD   IN_SOURCE  SegType ALPHA
 
Relation  1   PLANTS VENDOR   VENDORS VENDOR
 
Relation  2   PLANTS ITEM   ORDERS ITEM
 
Structure VENDORS   DBL ISAM
   Description "Vendor Record"
 
Tag FIELD   VEND_RTYPE EQ "1"
 
Field VEND_KEY   Type DECIMAL   Size 6
   Description "Vendor ID"
   Report Just LEFT   Input Just LEFT
 
Field VEND_RTYPE   Type DECIMAL   Size 1
   Description "Record Type"
   Report Just LEFT   Input Just LEFT
 
Field VEND_NAME   Type ALPHA   Size 30
   Description "Vendor Name"
 
Field VEND_STREET   Type ALPHA   Size 25
   Description "Street Address"
 
Field VEND_CITY   Type ALPHA   Size 20
   Description "City"
 
Field VEND_STATE   Type ALPHA   Size 2
   Description "State"
 
Field VEND_ZIP   Type DECIMAL   Size 9
   Description "Zip Code"
 
Field VEND_CONTACT   Type ALPHA   Size 25
   Description "Contact Person"
 
Field VEND_TELEPHONE   Template PHONE
   Description "Phone number"
   Report Heading "Telephone"
 
Field VEND_FAX   Template PHONE
   Description "Fax Number"
 
Field VEND_TERMS   Type ALPHA   Size 24
   Description "Terms & Conditions"
 
Key VENDOR   ACCESS   Order ASCENDING   Dups NO
   Segment FIELD   VEND_KEY
 
Key TAG_VENDOR   ACCESS   Order ASCENDING   Dups NO   Modifiable YES
   Description "Access key of tag+key"
   Segment FIELD   VEND_RTYPE  SegType ALPHA
   Segment FIELD   VEND_KEY  SegType ALPHA
 
Relation  1   VENDORS VENDOR   PLANTS VENDOR
 
Relation  2   VENDORS VENDOR   ORDERS VENDOR
 
File CUSTOMER   DBL ISAM   "ICSTUT:customer.ism"
   Description "Customer/Vendor File"
   Assign CUSTOMERS, VENDORS
 
File ORDERS   DBL ISAM   "ICSTUT:orders.ism"
   Description "Order File"
   Assign ORDERS
 
File PLANTS   DBL ISAM   "ICSTUT:plants.ism"
   Description "Plant Inventory File"
   Assign PLANTS
 
