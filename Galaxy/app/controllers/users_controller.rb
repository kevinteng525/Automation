class UsersController < ApplicationController
  before_action :set_user, only: [:show, :edit, :update, :destroy]
  #skip_before_filter :authorize
  # GET /users
  # GET /users.json
  def index
    @users = User.get_all_force_users
  end

  # GET /users/1
  # GET /users/1.json
  def show
    
  end

  # GET /users/new
  def new
    @user = User.new
  end

  # GET /users/1/edit
  def edit
    #hack the rails
    def @user.new_record?()
        false
    end
  end

  # POST /users
  # POST /users.json
  def create
    #@user = User.new(user_params)    
    respond_to do |format|
      if User.create_force_user(user_params)
        format.html { redirect_to users_path, notice: 'User was successfully created.' }
        format.json { render action: 'show', status: :created, location: @user }
      else
        format.html { render action: 'new' }
        format.json { render json: @user.errors, status: :unprocessable_entity }
      end
    end
  end

  # PATCH/PUT /users/1
  # PATCH/PUT /users/1.json
  def update
      respond_to do |format|   
        if User.update_force_user(params[:id],user_params)
          format.html { redirect_to @user, notice: 'User was successfully updated.' }
          format.json { head :no_content }
        else
          format.html { render action: 'edit' }
          format.json { render json: @user.errors, status: :unprocessable_entity }
        end
      end
  end

  # DELETE /users/1
  # DELETE /users/1.json
  def destroy
    User.delete_force_user(params[:id])
    respond_to do |format|
      format.html { redirect_to users_url }
      format.json { head :no_content }
    end
  end

  def subscribed_projects
    @subscribed_projects = User.get_subscribed_projects(params[:id])
  end

  private

    # Use callbacks to share common setup or constraints between actions.
    def set_user
      @user = User.get_force_user_by_id(params[:id])
    end

    # Never trust parameters from the scary internet, only allow the white list through.
    def user_params
      params.require(:user).permit(:name, :password, :user_type, :role, :is_active,:email, :description)      
    end
end
