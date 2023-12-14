import os
import re

REPLACEMENTS = [
    {
        "findFilename": r'(\w+)-01\.svg',
        "replaceFilename": r'\1.svg',
        "find": r'viewBox="([\d]+) ([\d]+) ([\d]+) ([\d]+)"',
        "replace": lambda match: f'viewBox="{int(match.group(1)) - 10} {int(match.group(2)) - 10} {int(match.group(3)) + 20} {int(match.group(4)) + 20}"'
    },
    {
        "findFilename": r'\w+\.svg',
        "find": "#000",
        "replace": "#fff"
    }
]

filenames = os.listdir()
for replacement in REPLACEMENTS:
    print(f"Running replacement on {replacement['findFilename']}.")
    for filename in filter(re.compile(replacement["findFilename"]).match, filenames):
        print(f"Rewriting {filename}.")
        with open(filename, "r") as file:
            data = file.read()
        print("Original:")
        print(data)
        data = re.sub(replacement["find"], replacement["replace"], data)
        print("New:")
        print(data)
        with open(filename, "w") as file:
            file.write(data)
        if "replaceFilename" in replacement:
            newFilename = re.sub(replacement["findFilename"], replacement["replaceFilename"], filename)
            print(f"Renaming {filename} to {newFilename}.")
            os.rename(filename, newFilename)
