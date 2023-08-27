import React, { Component } from 'react';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';

export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor (props) {
    super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.state = {
        collapsed: true,
        loggedIn: false,
        logoutUrl: "/bff/logout",
    };

    this.fetchIsUserLoggedIn = this.fetchIsUserLoggedIn.bind(this);
  }

  componentDidMount() {
      (async () => this.fetchIsUserLoggedIn())();
  }

  toggleNavbar () {
    this.setState({
      collapsed: !this.state.collapsed
    });
   }

  async fetchIsUserLoggedIn() {
      try {
          const response = await fetch("/bff/user", {
              headers: {
                  "X-CSRF": 1,
              },
          });

          if (response.ok && response.status === 200) {
              const data = await response.json();
              const logoutUrl =
                  data.find((claim) => claim.type === "bff:logout_url")?.value ??
                  this.state.logoutUrl;
              this.setState({ loggedIn: true, logoutUrl });
          }
      } catch (e) {
          console.error(e);
          this.setState({ loggedIn: false });
      }
  }

  render() {
    return (
      <header>
        <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" container light>
          <NavbarBrand tag={Link} to="/">MQTT.ClientReactApp</NavbarBrand>
          <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
          <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
            <ul className="navbar-nav flex-grow">
              <NavItem>
                <NavLink tag={Link} className="text-dark" to="/">Home</NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} className="text-dark" to="/counter">Counter</NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} className="text-dark" to="/fetch-data">Fetch data</NavLink>
                        </NavItem>
              <NavItem>
                          {/*<NavLink tag={Link} className="text-dark" to="/user-session">Login</NavLink>*/}
                          {this.state.loggedIn && (
                              <NavItem>
                                  <NavLink
                                      tag={Link}
                                      className="text-dark"
                                      to="/user-session"
                                  >
                                      Show User Session
                                  </NavLink>
                              </NavItem>
                            )}
                            <NavItem>
                                <a
                                    className="text-dark nav-link"
                                    href={
                                        this.state.loggedIn ? this.state.logoutUrl : "/bff/login"
                                    }
                                >
                                    {this.state.loggedIn ? "Logout" : "Login"}
                                </a>
                            </NavItem>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} className="text-dark" to="/back-channel-area">Back Channel</NavLink>
              </NavItem>
            </ul>
          </Collapse>
        </Navbar>
      </header>
    );
  }
}
