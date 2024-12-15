import sqlite3
import argparse

def run_sql_command(file_pass : str, command : str) -> str:
    connection = sqlite3.connect(file_pass)
    cursor = connection.cursor()
    result = str(cursor.execute(command))
    connection.commit()
    connection.close()
    return result

def main():
    parser = argparse.ArgumentParser(description="Database Viewer")
    parser.add_argument('function', choices=['run_sql_command'], help='Function to execute')
    parser.add_argument('params', nargs='*', help='Parameters for the function')
    args = parser.parse_args()

    if args.function == 'run_sql_command':
        if len(args.params) != 1:
            print("Usage: python creator.py run_sql_command <command>")
        else:
            file_path = args.params[0]
            command = args.params[1]
            result = run_sql_command(file_path, command)
            print(result)

if __name__ == '__main__':
    main()