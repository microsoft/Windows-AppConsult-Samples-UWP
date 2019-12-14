import { NativeModules } from 'react-native';

export const getResult = () => {
    return new Promise ((resolve, reject) => {
        NativeModules.SampleComponent.getResult(function(result, error) {
            if (error) {
                return reject(error);
            }
            else {
                resolve(result);
            }
        })
    })
}

export const getDeviceModel = () => {
    return new Promise((resolve, reject) => {
        NativeModules.SampleComponent.getDeviceModel(function(result, error) {
            if (error) {
                reject(error);
            }
            else {
                resolve(result);
            }
        })
    })
}

export const doSomething = () => {
    return 'Hello world'
}
