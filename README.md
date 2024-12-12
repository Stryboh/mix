# mix

# pip install sqlite3

# 

add_data_to_table(file_path : str, table_name : str, columns : list, data : tuple[list, ], condition : str = '') -> str | None

adds several rows of data into specified rows in the table

input:
path to database, title of table, list titles of columns to place data, data to insert, specification of rows

output:
error string or None

# 

erase_data_from_table(file_path : str, table_name : str, condition : str = '') -> str | None

removes specified rows in the table

input:
path to database, title of table, specification of rows

output:
error string or None

# 

modify_data_in_table(file_path : str, table_name : str, column : str, value : str, condition : str = '') -> str | None

replaces data in the table

input:
path to database, title of table, title of column, new value, specification of rows

output:
error string or None

# 

set_data_none_in_table(file_path : str, table_name : str, column : str, condition : str = '') -> str | None

sets data to None value in the table

input:
path to database, title of table, title of column, specification of rows

output:
error string or None
