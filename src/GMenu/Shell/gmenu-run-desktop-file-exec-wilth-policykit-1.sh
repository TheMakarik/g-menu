#!/bin/sh
#=================
# GMENU - simple .desktop file editor for GNU/Linux
#=================

COMMAND="$@"
echo "Args: $@"

if command -v pkexec > /dev/null 2>&1; then
    # Сохраняем переменные окружения для GUI
    pkexec env DISPLAY=$DISPLAY XAUTHORITY=$XAUTHORITY DBUS_SESSION_BUS_ADDRESS=$DBUS_SESSION_BUS_ADDRESS $COMMAND
else
    sudo -E $COMMAND
fi