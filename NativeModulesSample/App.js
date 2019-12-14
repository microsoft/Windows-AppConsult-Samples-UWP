/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 *
 * @format
 * @flow
 */

import React, {Fragment} from 'react';
import {
  StyleSheet,
  View,
  Text,
  StatusBar,
  Button
} from 'react-native';


import * as SampleComponent from './SampleComponent'

class App extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      model: '',
    }
  }

  getModel = async () => {
    var model = await SampleComponent.getDeviceModel();
    this.setState( { model: model});
  }

  render() {
    return (
      <View style={styles.sectionContainer}>
        <StatusBar barStyle="dark-content" />
        <View>
          <Button title="Get model" onPress={this.getModel} />
          <Text>Model: {this.state.model}</Text>
        </View>
      </View>
    );
  }
 
};

const styles = StyleSheet.create({
  scrollView: {
  },
  engine: {
    position: 'absolute',
    right: 0,
  },
  body: {
  },
  sectionContainer: {
    marginTop: 32,
    paddingHorizontal: 24,
  },
  sectionTitle: {
    fontSize: 24,
    fontWeight: '600',
  },
  sectionDescription: {
    marginTop: 8,
    fontSize: 18,
    fontWeight: '400',
  },
  highlight: {
    fontWeight: '700',
  },
  footer: {
    fontSize: 12,
    fontWeight: '600',
    padding: 4,
    paddingRight: 12,
    textAlign: 'right',
  },
});

export default App;
