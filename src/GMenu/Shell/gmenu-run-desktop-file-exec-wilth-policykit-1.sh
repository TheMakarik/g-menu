#!/bin/sh
#=================
# GMENU - запуск команды с pkexec (сохраняя окружение)
#=================

COMMAND="$1"

if [ -z "$COMMAND" ]; then
    echo "No command specified" >&2
    exit 1
fi

if echo "$COMMAND" | grep -q "^'.*'$"; then
    COMMAND=$(echo "$COMMAND" | sed "s/^'//;s/'$//")
fi


if echo "$COMMAND" | grep -q '^".*"$'; then
    COMMAND=$(echo "$COMMAND" | sed 's/^"//;s/"$//')
fi

if command -v pkexec > /dev/null 2>&1; then
    ESCAPED_CMD=$(printf "%s" "$COMMAND" | sed "s/'/'\\\\''/g")
    pkexec sh -c "DISPLAY='$DISPLAY' XAUTHORITY='$XAUTHORITY' DBUS_SESSION_BUS_ADDRESS='$DBUS_SESSION_BUS_ADDRESS' XDG_RUNTIME_DIR='$XDG_RUNTIME_DIR' WAYLAND_DISPLAY='$WAYLAND_DISPLAY' exec sh -c '$ESCAPED_CMD'"
elif command -v sudo > /dev/null 2>&1; then
    sudo -E sh -c "$COMMAND"
else
    echo "Error: Neither pkexec nor sudo found" >&2
    exit 1
fi