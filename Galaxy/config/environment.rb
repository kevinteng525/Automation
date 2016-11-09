# Load the Rails application.
require File.expand_path('../application', __FILE__)

#global functions, timeout 5 minutes
def force_server
  RestClient::Resource.new(Galaxy::Application::FORCE_SERVER, :headers=>{:accept => :json}, :timeout=>300000,:open_timeout=>300000)
end

def ms_json_to_date(d)
  if d
    Time.at(d.split('(')[1].to_i/1000).to_s
  else    
    nil
  end
end

def db_date_to_rails_date(d)
  #1/13/2015 to 13/1/2015
  segments = d.split('/')
  segments[1]+'/'+segments[0]+'/'+segments[2]
end

def db_time_to_rails_time(t)
  segments = t.split('/')
  segments[1]+'/'+segments[0]+'/'+segments[2]
end
# Initialize the Rails application.
Galaxy::Application.initialize!
