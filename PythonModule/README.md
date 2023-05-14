# Installation
 - Install python https://www.python.org/downloads/
 - Install necessary packages with **pip install -r requirementes.txt**
 - Run API python main.py
   - By deafault api runs on 8000 port
   - To make post requests body should conatin "score" key and array socres for last 5 years e.g
    
    {
      "scores": [1632.0, 1099.0, 1773.0, 1737.0, 1899.0]
    }