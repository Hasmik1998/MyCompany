import pickle
from flask import request, Flask
from sklearn.linear_model import LogisticRegression

loaded_model = pickle.load(open("C:\\Users\\hasmik.petrosyan\\Desktop\\Capstone\\PythonModule\\logit.sav", 'rb'))

app = Flask(__name__)

@app.route('/predict', methods=['POST'])
def predict():
    request_data:dict = request.get_json()
    user_socer_history = [ float(score) for score in request_data["scores"]]
    user_prediction = loaded_model.predict([user_socer_history]).tolist()
    return {"prediction": user_prediction}

if __name__ == '__main__':
    app.run(port=8000)