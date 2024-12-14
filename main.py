import credentials
import asyncio
import logging
import sys
import sqlite3

from aiogram import Bot, Dispatcher, html
from aiogram.client.default import DefaultBotProperties
from aiogram.enums import ParseMode
from aiogram.filters import CommandStart, Command
from aiogram.types import Message

TOKEN = credentials.token()
database_title = "credentials.db"

dp = Dispatcher()

def get_credentials_by_code(code: str, file_path: str = database_title) -> tuple[str, str] | None:
    connection = sqlite3.connect(file_path)
    cursor = connection.cursor()
    data = cursor.execute("SELECT login, password FROM Users WHERE code=?", (code,)).fetchone()
    connection.close()
    if data:
        login, password = data
        return login, password
    else:
        return None

def check_admin(id: int, file_path: str = database_title) -> bool:
    connection = sqlite3.connect(file_path)
    cursor = connection.cursor()
    is_admin = cursor.execute(f"SELECT id FROM Admins WHERE id={id}").fetchall()
    connection.close()
    if is_admin:
        return True
    else:
        return False

@dp.message(CommandStart())
async def command_start_handler(message: Message) -> None:
    await message.answer(f"Привет, {html.bold(message.from_user.full_name)}! Отправь мне кодовое число для доступа.")


@dp.message(Command("add"))
async def add_admin_handler(message: Message) -> None:
    if not check_admin(message.from_user.id):
        print(message.from_user.id)
        await message.answer("Вы не являетесь администратором! -_-")
        return

    code, login, password, role = message.text.split(' ')[1:]
    connection = sqlite3.connect(database_title)
    cursor = connection.cursor()
    cursor.execute(f"INSERT INTO Users (code, login, password, role) VALUES ({code}, '{login}', '{password}', '{role}')")
    connection.commit()
    connection.close()
    await message.answer(f"{role} добавлен")


@dp.message(Command("erase"))
async def erase_handler(message: Message) -> None:
    if not check_admin(message.from_user.id):
        print(message.from_user.id)
        await message.answer("Вы не являетесь администратором! -_-")
        return

    code = message.text.split(' ')[1]
    connection = sqlite3.connect(database_title)
    cursor = connection.cursor()
    cursor.execute(f"DELETE FROM Users WHERE code='{code}'")
    connection.commit()
    connection.close()
    await message.answer(f"Пользователь с ID {code} удалён")


@dp.message(Command("list"))
async def users_list_handler(message: Message) -> None:
    if not check_admin(message.from_user.id):
        print(message.from_user.id)
        await message.answer("Вы не являетесь администратором! -_-")
        return

    connection = sqlite3.connect(database_title)
    cursor = connection.cursor()
    data = cursor.execute(f"SELECT code, login, role FROM Users").fetchall()
    connection.close()

    line = ""
    for row in data:
        line += f"{str(row[0])}  {str(row[1])}  {str(row[2])}\n"
    if line == "":
        line = "Нет записей"
    await message.answer(line)


@dp.message()
async def check_code_handler(message: Message) -> None:
    code = message.text.strip()
    credentials = get_credentials_by_code(code)
    if credentials:
        login, password = credentials
        await message.answer(f"Логин: {html.bold(login)}\nПароль: {html.bold(password)}")
    else:
        await message.answer("Кодовое число не найдено. Попробуйте снова.")


async def main() -> None:
    bot = Bot(token=TOKEN, default=DefaultBotProperties(parse_mode=ParseMode.HTML))
    await dp.start_polling(bot)


if __name__ == "__main__":
    logging.basicConfig(level=logging.INFO, stream=sys.stdout)
    asyncio.run(main())

