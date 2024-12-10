import sqlite3

def add_data_to_table(file_path : str, table_name : str, columns : list, data : tuple[list, ], condition : str = '') -> str | None:
    try:
        connection = sqlite3.connect(file_path)
        cursor = connection.cursor()

        column_list = "("
        for column in columns:
            column_list += str(column) + ", "
        column_list = column_list[:-2] + ")"

        data_list = "("
        for dat in data:
            data_list += str(dat) + ",\n("
            for i in range(len(dat)):
                data_list += str(dat[i]) + ", "
            data_list = data_list[:-2] + ")"
        cursor.execute("INSERT INTO " + table_name + " " + column_list + " VALUES " + data_list + "WHERE " + condition)
        connection.close()
        return
    except sqlite3.Error as error:
        return str(error)

def erase_data_from_table(file_path : str, table_name : str, condition : str = '') -> str | None:
    try:
        connection = sqlite3.connect(file_path)
        cursor = connection.cursor()
        cursor.execute("DELETE FROM " + table_name + " WHERE " + condition)
        connection.close()
        return
    except sqlite3.Error as error:
        return str(error)

def modify_data_in_table(file_path : str, table_name : str, column : str, value : str, condition : str = '') -> str | None:
    try:
        connection = sqlite3.connect(file_path)
        cursor = connection.cursor()
        cursor.execute("UPDATE " + table_name + " SET " + column + "=" + value + " WHERE " + condition)
        connection.close()
        return
    except sqlite3.Error as error:
        return str(error)

def set_data_none_in_table(file_path : str, table_name : str, column : str, condition : str = '') -> str | None:
    try:
        connection = sqlite3.connect(file_path)
        cursor = connection.cursor()
        cursor.execute("UPDATE " + table_name + " SET " + column + "=NULL WHERE " + condition)
        connection.close()
        return
    except sqlite3.Error as error:
        return str(error)
