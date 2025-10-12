// === Тест ядра команд: ls, cd, pwd, date, cal и VFS ===

/* Инициализация VFS */
vfs-init vfs/minimal.json

/* pwd в корне */
pwd

/* ls по умолчанию (текущая директория) */
ls

/* ls по относительному и абсолютному путям */
ls home
ls /etc

/* cd в home и проверка pwd */
cd home
pwd
ls

/* переходы назад и по относительному пути */
cd ..
pwd
cd /etc
pwd

/* ошибки: несуществующий путь и попытка зайти в файл как в каталог */
cd /nope
ls /nope
cd /etc/config.ini

/* date без аргументов и с форматом */
date
date +yyyy-MM-dd_HH:mm

/* cal на текущий и на конкретный месяц/год */
cal
cal 10 2025

/* сброс VFS и проверка пустого ls */
vfs-init
ls

exit 0