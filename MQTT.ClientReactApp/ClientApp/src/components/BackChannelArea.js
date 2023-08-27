import React, { Component } from "react";

export class BackChannelArea extends Component {
  static displayName = BackChannelArea.name;

  constructor(props) {
    super(props);
    this.state = { loading: true };
  }

  componentDidMount() {
      this.fetchJwtTokenInfo();  
  }

    static renderUserSessionTable(userSession) {
        
    }
   

  render() {
      <div className="table table-striped" aria-labelledby="tabelLabel">
          <p>Getting token from back channel...</p>
      </div>

    return (
      <div>
        <h1 id="tabelLabel">Back channel</h1>
        <p>This pages shows nothing.</p>
      </div>
    );
  }

  async fetchJwtTokenInfo() {
      const response = await fetch("jwttoken", {
      headers: {
        "X-CSRF": 1,
      },
    });
    const data = await response.json();
    this.setState({ loading: false });
  }
}
