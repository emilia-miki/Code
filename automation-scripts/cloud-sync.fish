#!/opt/homebrew/bin/fish

/opt/homebrew/bin/rclone sync /Users/miki/files/ mega:/ --exclude /code/ --exclude /office/
/opt/homebrew/bin/rclone sync onedrive:/ /Users/miki/files/office/

