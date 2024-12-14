import sqlite3

def add_data_to_table(file_path : str, table_name : str, columns : list, data : list[list, ]) -> str | None:
    try:
        connection = sqlite3.connect(file_path)
        cursor = connection.cursor()

        column_list = "("
        for column in columns:
            column_list += str(column) + ", "
        column_list = column_list[:-2] + ")"

        data_list = ""
        for dat in data:
            data_list += "("
            for i in range(len(dat)):
                data_list += str(dat[i]) + ", "
            data_list = data_list[:-2] + "), "
        data_list = data_list[:-2]
        action = "INSERT INTO " + table_name + " " + column_list + " VALUES " + data_list
        print(action)
        cursor.execute(action)
        connection.commit()
        connection.close()
        return
    except sqlite3.Error as error:
        return str(error)

def erase_data_from_table(file_path : str, table_name : str, condition : str = '') -> str | None:
    try:
        connection = sqlite3.connect(file_path)
        cursor = connection.cursor()
        cursor.execute("DELETE FROM " + table_name + " WHERE " + condition)
        connection.commit()
        connection.close()
        return
    except sqlite3.Error as error:
        return str(error)

def modify_data_in_table(file_path : str, table_name : str, column : str, value : str, condition : str = '') -> str | None:
    try:
        connection = sqlite3.connect(file_path)
        cursor = connection.cursor()
        cursor.execute("UPDATE " + table_name + " SET " + column + "=" + value + " WHERE " + condition)
        connection.commit()
        connection.close()
        return
    except sqlite3.Error as error:
        return str(error)

def set_data_none_in_table(file_path : str, table_name : str, column : str, condition : str = '') -> str | None:
    try:
        connection = sqlite3.connect(file_path)
        cursor = connection.cursor()
        cursor.execute("UPDATE " + table_name + " SET " + column + "=NULL WHERE " + condition)
        connection.commit()
        connection.close()
        return
    except sqlite3.Error as error:
        return str(error)


print(add_data_to_table("your_database.db", "your_table", ["id", "your_column"], [[14, "'abc'"], [88, "'88'"]]))
print()
print()
print()
print(erase_data_from_table("your_database.db", "your_table", "id=88"))
print()
print()
print()
print(modify_data_in_table("your_database.db", "your_table", "id", "69", "id=14"))
print()
print()
print()
print(set_data_none_in_table("your_database.db", "your_table", "your_column", "id=69"))
