class LoginController < ApplicationController
  skip_before_filter :authorize
def new
  @redirect_path = params[:path]
end

def create
  user = User.authenticate(params[:name], params[:password])
  if user
    session[:user_id] = user.id
    session[:user_role] = user.role
    #redirect to the original url user requested
    redirect_to params[:path]!='' ? params[:path] : root_path
  else
    redirect_to login_url, :alert=>'Invalid user/password combination!'
  end
end

def destroy
  session[:user_id] = nil
  session[:user_role] = nil
  redirect_to login_path, :notice => 'logged out!'
end
  
  
end
