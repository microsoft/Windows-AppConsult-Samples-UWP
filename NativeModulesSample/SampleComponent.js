import { NativeModules } from 'react-native';

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
