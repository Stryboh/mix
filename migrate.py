import sqlite3


def intersection(list1 : list, list2 : list) -> list:
    result = []
    for item in list1:
        try:
            list2.index(item)
            result.append(item)
        except None:
            pass
    return result


def get_database_tables(file_path: str) -> list | None:
    connection = sqlite3.connect(file_path)
    cursor = connection.cursor()
    cursor.execute("SELECT name FROM sqlite_master WHERE type='table'")
    tables = cursor.fetchall()
    connection.close()
    return tables


def get_table_columns(file_path : str, table_name : str) -> list | None:
    connection = sqlite3.connect(file_path)
    cursor = connection.cursor()
    data = cursor.execute(f"SELECT name, type FROM pragma_table_info('{table_name}')").fetchall()
    connection.close()
    return data


def check_connectivity(file_1_path: str, table_1_name: str, column_1_name: str, file_2_path: str, table_2_name: str, column_2_name: str) -> bool:
    try:
        connection1 = sqlite3.connect(file_1_path)
        cursor1 = connection1.cursor()
        type1 = cursor1.execute(f"SELECT type FROM pragma_table_info('{table_1_name}') WHERE name='{column_1_name}'").fetchall()
        connection1.close()

        connection2 = sqlite3.connect(file_2_path)
        cursor2 = connection2.cursor()
        type2 = cursor2.execute(f"SELECT type FROM pragma_table_info('{table_2_name}') WHERE name='{column_2_name}'").fetchall()
        connection2.close()

        return type1 == type2
    except None:
        return False


def transfer_data(old_file_path: str, old_table_name: str, old_column_name: str, new_file_path: str, new_table_name: str, new_column_name: str) -> str | None:
    if check_connectivity(old_file_path, old_table_name, old_column_name, new_file_path, new_table_name, new_column_name):
        connection1 = sqlite3.connect(old_file_path)
        connection2 = sqlite3.connect(new_file_path)
        cursor1 = connection1.cursor()
        cursor2 = connection2.cursor()
        try:
            new_column_data = cursor1.execute(f"SELECT {old_column_name} FROM {old_table_name}").fetchall()
            rows = len(cursor2.execute(f"SELECT {new_column_name} FROM {new_table_name}").fetchall())
            for i in range(len(new_column_data) - rows):
                cursor2.execute(f"INSERT INTO {new_table_name} ({new_column_name}) VALUES (null)")
            action = f"UPDATE {new_table_name} SET {new_column_name}=CASE "
            for i in range(len(new_column_data)):
                action += f"WHEN rowid={i+1} THEN {new_column_data[i][0]} "
            action += f" END"
            cursor2.execute(action)

            connection2.commit()
            connection1.close()
            connection2.close()
        except sqlite3.Error as error:
            connection1.close()
            connection2.close()
            return str(error)
    else:
        return "Columns are not compatible"


def auto_transfer_data(old_file_path : str, new_file_path : str) -> list:
    old_tables = get_database_tables(old_file_path)
    new_tables = get_database_tables(new_file_path)

    old_columns = []
    new_columns = []

    for table in old_tables:
        columns = get_table_columns(old_file_path, table[0])
        for column in columns:
            old_columns.append((table[0], (column[0], column[1])))

    for table in new_tables:
        columns = get_table_columns(new_file_path, table[0])
        for column in columns:
            new_columns.append((table[0], (column[0], column[1])))

    columns = intersection(old_columns, new_columns)
    columns_copied = []
    for column in columns:
        table_name, column_data = column
        column_name, column_type = column_data
        if check_connectivity(old_file_path, table_name, column_name, new_file_path, table_name, column_name):
            columns_copied.append(transfer_data(old_file_path, table_name, column_name, new_file_path, table_name, column_name))
    return columns_copied
