call "C:\WPy32-3760\scripts\env_for_icons.bat"

copy ..\data\covidsp.csv .\dataset /Y

kaggle datasets version -d -m "Atualizacao Automatica" -p ./dataset

pause 



